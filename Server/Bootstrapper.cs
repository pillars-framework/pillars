#pragma warning disable CS0618 // Type or member is obsolete
namespace Pillars;

public sealed class Bootstrapper
{
	private readonly IHost _host;
	private static readonly ILogger _logger = Log.ForContext<Bootstrapper>();

	private AppSettings? _appSettings;

	public Bootstrapper()
	{
		var builder = Host.CreateDefaultBuilder();

		builder.UseContentRoot(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!)
			.ConfigureAppConfiguration(ConfigureJsons)
			.ConfigureLogging(ConfigureLogging)
			.ConfigureServices(ConfigureServices);

		_host = builder.Build();
	}

	/// <summary>
	/// Loads all "*settings.json" from the server directory and adds them to the configuration builder.
	/// </summary>
	private void ConfigureJsons(HostBuilderContext context, IConfigurationBuilder configurationBuilder)
	{
		var jsonFiles = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*settings.json",
			SearchOption.TopDirectoryOnly);
		foreach (var json in jsonFiles)
			configurationBuilder.AddJsonFile(json, optional: false, reloadOnChange: true);
	}

	/// <summary>
	/// Clear the EventLogin because it's not compatible with this usecase
	/// Sets the Serilog internal logger to our configured logger instance
	/// </summary>
	private void ConfigureLogging(HostBuilderContext context, ILoggingBuilder loggingBuilder)
	{
		_appSettings = context.Configuration.GetSection(nameof(AppSettings)).Get<AppSettings>();
		if (_appSettings is null) throw new("AppSettings is null");

		//Add appsettings to the DI container
		loggingBuilder.Services.AddSingleton(_appSettings);

		// Clear existing providers because they are not supported for our usecase
		loggingBuilder.ClearProviders();

		// Configures the logger for the console
		var logger = new LoggerConfiguration()
			.ReadFrom.Configuration(context.Configuration, "Serilog",
				ConfigurationAssemblySource.AlwaysScanDllFiles)
			.MinimumLevel
			.ControlledBy(new(_appSettings.IsDebug ? LogEventLevel.Debug : LogEventLevel.Information))
			.CreateLogger();

		Log.Logger = new LoggerConfiguration()
			.MinimumLevel.Verbose()
			.WriteTo.Logger(logger)
			.CreateLogger();

		_logger.Information("Logger & AppSettings configured, injecting additional settings ...");

		var settingTypes = Assembly.GetExecutingAssembly().GetTypes()
			.Where(t => t.GetInterfaces().Contains(typeof(ISettings))).ToList();
		foreach (var settingType in settingTypes)
		{
			var setting = context.Configuration.GetSection(settingType.Name).Get(settingType);
			if (setting is not null) loggingBuilder.Services.AddSingleton(settingType, setting);
		}

		_logger.Information("... Done !");
	}

	/// <summary>
	/// Adds all our necessary services to the DI container
	/// </summary>
	private void ConfigureServices(HostBuilderContext context, IServiceCollection services)
	{
		services.AddOptions();
		services.AddSingleton(sp => sp);
		services.AddSingleton(Log.Logger);
		services.AutoRegister();
		services.BuildServiceProvider();
	}

	/// <summary>
	/// Scans for all registered singletons inside this assembly
	/// and instantiates them for future use
	/// </summary>
	private void ScanForAttributeInstantiation()
	{
		foreach (var type in Assembly.GetExecutingAssembly().GetTypes()
			         .Where(t => t.GetCustomAttribute<RegisterSingletonAttribute>() is not null))
			_ = _host.Services.GetRequiredService(type);
	}

	/// <summary>
	/// Initializes the DI container , searches for all attributes
	/// that need to be instantiated and runs the current host
	/// </summary>
	public async Task<Task> RunAsync()
	{
		try
		{
			_logger.Information("DI container initialized, starting host...");
			// Ensure Database is always initialized
			var db = _host.Services.GetRequiredService<DatabaseController>();
			await db.InitializeAsync();
			_logger.Information("Initialized {c}", db.GetType().Name);
			// Initialize all other IControllers
			await DependencyGraphHelper.ResolveControllerInitialization(_host.Services);
			// Instantiate all other services
			ScanForAttributeInstantiation();
			// Emit signal to distribute initialization completion
			_host.Services.GetRequiredService<ServerController>().ServerInitialized();
			// Runs the host asynchronously and continues with exiting the environment with a code of -1
			return _host.RunAsync().ContinueWith(_ => Environment.Exit(-1));
		}
		catch (Exception e)
		{
			_logger.Error(e, "An error occurred during bootstrapping");
		}

		Environment.Exit(1); // Force Shutdown
		return Task.CompletedTask;
	}
}

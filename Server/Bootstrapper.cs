#pragma warning disable CS0618 // Type or member is obsolete
namespace Pillars;

public sealed class Bootstrapper
{
    private readonly IHost _host;
    private static readonly ILogger _logger = Log.ForContext<Bootstrapper>();
    
    private AppSettings? _appSettings { get; set; }

    public Bootstrapper()
    {
        var builder = Host.CreateDefaultBuilder();

        builder.UseContentRoot(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!)
            .ConfigureAppConfiguration(ConfigureJsons)
            .ConfigureLogging(ConfigureLogging)
            .ConfigureServices(ConfigureServices);
        
        _host = builder.Build();
    }

    private void ConfigureJsons(HostBuilderContext context, IConfigurationBuilder configurationBuilder)
    {
        var jsonFiles = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*settings.json", SearchOption.TopDirectoryOnly);

        foreach (var json in jsonFiles)
        {
            configurationBuilder.AddJsonFile(json, optional: false, reloadOnChange: true);
        }
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
        
        var settingTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.GetInterfaces().Contains(typeof(ISettings))).ToList();

        //TODO:: fix settings not loaded correctly cause jsons are split now and not in one file
        foreach (var settingType in settingTypes)
        {
            var setting = context.Configuration.GetSection(nameof(settingType)).Get(settingType);
            if (setting != null) 
                loggingBuilder.Services.AddSingleton(setting);
        }

        //Clear existing providers because they are not supported for our usecase
        loggingBuilder.ClearProviders();

        // Configures the logger for the console
        var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(context.Configuration, "Serilog", ConfigurationAssemblySource.AlwaysScanDllFiles)
            .MinimumLevel.ControlledBy(new(_appSettings.IsDebug ? LogEventLevel.Debug : LogEventLevel.Information))
            .CreateLogger();

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Logger(logger)
            .CreateLogger();
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
        foreach (var type in Assembly.GetExecutingAssembly().GetTypes().Where(t => t.GetCustomAttribute<RegisterSingletonAttribute>() is not null)) 
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
            await DependencyGraphHelper.ResolveControllerInitialization(_host.Services);
            ScanForAttributeInstantiation();

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

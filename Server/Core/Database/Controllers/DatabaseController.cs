namespace Pillars.Core.Database.Controllers;

[RegisterSingleton]
public sealed class DatabaseController(ILogger logger, DatabaseSettings settings)
{
	private readonly ILogger _logger = logger.ForThisContext();

	/// <summary>
	/// Initializes the database and applies configuration
	/// from the database settings.
	/// </summary>
	public async Task InitializeAsync()
	{
		try
		{
			MongoClientSettings clientsettings = new() { Server = new(settings.Host, settings.Port) };

			// Apply optional credential
			if (settings.Credentials is not null)
				clientsettings.Credential =
					MongoCredential.CreateCredential(settings.Database,
						settings.Credentials.User, settings.Credentials.Password);

			// Enable debug log, if set
			if (settings.DebugLog)
				clientsettings.ClusterConfigurator = cb =>
				{
					cb.Subscribe<CommandStartedEvent>(e =>
						_logger.Debug("{command}", e.Command.ToJson()));
				};

			await DB.InitAsync(settings.Database, clientsettings);
		}
		catch (Exception e)
		{
			_logger.Error(e);
		}
	}
}

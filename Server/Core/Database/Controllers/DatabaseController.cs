namespace Pillars.Core.Database.Controllers;

using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;

[RegisterSingleton(typeof(DatabaseController))]
public sealed class DatabaseController(ILogger logger, DatabaseSettings settings) : IController
{
	private readonly ILogger _logger = logger.ForThisContext();

	public async Task InitializeAsync()
	{
		try
		{
			await DB.InitAsync(settings.Database, settings.Host, settings.Port);
		}
		catch (Exception e)
		{
			_logger.Error(e);
		}
	}
}

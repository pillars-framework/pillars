namespace Pillars.Core.Server.Controllers;

[RegisterSingleton]
public sealed class ServerController
{
	private readonly ILogger _logger;
	private readonly ServerEvents _serverEvents;
	public readonly DateTime StartTime = DateTime.UtcNow;

	/// <summary>
	/// Current state of the server
	/// </summary>
	public SERVERSTATE ServerState { get; private set; } = SERVERSTATE.STARTING;

	public ServerController(ILogger l, ServerEvents se)
	{
		_logger = l.ForThisContext();
		_serverEvents = se;
		HogWarpConfig = LoadHogWarpConfig() ?? throw new("Could not load HogWarpConfig");
	}

	#region HogWarp

	public readonly HogWarpConfig HogWarpConfig;

	/// <summary>
	/// Tries to load the hogwarp server config from the config.json.
	/// Returns null in case 'config.json' could not be found or deserialized correctly.
	/// </summary>
	private HogWarpConfig? LoadHogWarpConfig()
	{
		try
		{
			var configFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
			if (!File.Exists(configFile))
			{
				_logger.Error($"Config file not found: {configFile}");
				return null;
			}

			var config = JsonSerializer.Deserialize<HogWarpConfig>(File.ReadAllText(configFile));
			if (config is null)
				_logger.Error($"Failed to deserialize config / config is empty: {configFile}");

			return config;
		}
		catch (Exception e)
		{
			_logger.Error(e);
			return null;
		}
	}

	#endregion

	/// <summary>
	/// Called when the server initialized completely.
	/// Fires signal. Puts the server into "Running" state from starting
	/// </summary>
	public async Task ServerInitialized()
	{
		if (ServerState != SERVERSTATE.STARTING) return;
		ServerState = SERVERSTATE.RUNNING;
		_logger.Information("Server initialization completed - now in state {state}",
			ServerHelper.GetServerStateAsString(ServerState));
		_serverEvents.ServerStarted();
	}
}

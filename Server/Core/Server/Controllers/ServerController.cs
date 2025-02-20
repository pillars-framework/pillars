namespace Pillars.Core.Server.Controllers;

[RegisterSingleton]
public sealed class ServerController(ILogger l, ServerEvents se)
{
	private readonly ILogger _logger = l.ForThisContext();
	public readonly DateTime StartTime = DateTime.UtcNow;
	/// <summary>
	/// Current state of the server
	/// </summary>
	public SERVERSTATE ServerState { get; private set; } = SERVERSTATE.STARTING;

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
		se.ServerStarted();
	}
}

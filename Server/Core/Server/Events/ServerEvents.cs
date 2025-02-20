namespace Pillars.Core.Server.Events;

[RegisterSingleton]
public sealed class ServerEvents
{
	#region ServerStarted
	public delegate Task ServerStartedDelegate();
	public event ServerStartedDelegate? OnServerStarted;
	/// <summary>
	/// Custom signal when the server started
	/// and successfully initialized
	/// </summary>
	public void ServerStarted() => OnServerStarted?.Invoke();
	#endregion

	#region ServerShutdown
	public delegate Task ServerShutdownDelegate();
	public event ServerShutdownDelegate? OnServerShutdown;
	/// <summary>
	/// Event signal that is awaited when the server
	/// needs to be gracefully shutdown, e.g. on performing a restart.
	/// </summary>
	public void ServerShutdown() => OnServerShutdown?.Invoke();
	#endregion
}

namespace Pillars.Core.Player.Events;

/// <summary>
/// Player connection events that are triggered by the server when a player connects or disconnects.
/// </summary>
[RegisterSingleton]
public sealed class PlayerConnectionEvents
{
	#region CONNECT

	/// <summary>
	/// Custom event signal that includes the player
	/// </summary>
	public delegate Task PlayerConnectedDelegate(PiPlayer player);

	/// <summary>
	/// Event that is fired if a player connects
	/// </summary>
	public event PlayerConnectedDelegate? OnPlayerConnected;

	/// <summary>
	/// Invokes the player connected event
	/// </summary>
	public void PlayerConnected(PiPlayer player) =>
		OnPlayerConnected?.Invoke(player);

	#endregion

	#region DISCONNECT

	/// <summary>
	/// Custom event signal with the invalidated PiPlayer
	/// </summary>
	public delegate Task PlayerDisconnectedDelegate(PiPlayer player);

	/// <summary>
	/// Event that is fired if a player disconnects
	/// </summary>
	public event PlayerDisconnectedDelegate? OnPlayerDisconnected;

	/// <summary>
	/// Invokes the player connected event
	/// </summary>
	public void PlayerDisconnected(PiPlayer player) =>
		OnPlayerDisconnected?.Invoke(player);

	#endregion
}

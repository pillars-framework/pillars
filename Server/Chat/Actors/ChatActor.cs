namespace Pillars.Chat.Actors;

[RegisterSingleton]
public sealed class ChatActor : PiActor<BpHogWarpChat>
{
	private readonly ChatEvents _chatEvents;
	private readonly PlayerController _playerController;

	public ChatActor(ChatEvents ce, PlayerController pc)
	{
		_chatEvents = ce;
		_playerController = pc;
		_worldActor.ParentActor = this;
	}

	#region SEND

	/// <summary>
	/// Sends a message to a specific player by forwarding it to the world actor's ReceiveMsg method.
	/// </summary>
	/// <param name="target">The player to whom the message is sent.</param>
	/// <param name="message">The message to send.</param>
	public async Task SendMessage(PiPlayer target, string message) =>
		_worldActor.RecieveMsg(target.Native, message);

	/// <summary>
	/// Sends a message to a set oof players by forwarding it to the world actor's ReceiveMsg method.
	/// </summary>
	/// <param name="targets">The players to whom the message is sent.</param>
	/// <param name="message">The message to send.</param>
	public async Task SendMessage(IEnumerable<PiPlayer> targets, string message)
	{
		foreach (var target in targets)
			SendMessage(target, message);
	}

	/// <summary>
	/// Broadcasts a message to all players by forwarding it to the world actor's ReceiveMsg method.
	/// </summary>
	/// <param name="message">The message to send.</param>
	public async Task BroadcastMessage(string message)
	{
		foreach (var player in _playerController.Players.Values)
			SendMessage(player, message);
	}

	#endregion

	#region RECEIVE

	/// <summary>
	/// Handles a received message from a player and forwards it to the chat events system.
	/// </summary>
	/// <param name="player">The player who sent the message.</param>
	/// <param name="message">The message sent by the player.</param>
	public void PlayerMessageReceived(NativePlayer player, string message)
	{
		if (!_playerController.Players.TryGetValue(player, out var piPlayer)) return;
		_chatEvents.ChatMessage(piPlayer, message);
	}

	#endregion
}

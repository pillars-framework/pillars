namespace Pillars.Chat.Controllers;

[RegisterSingleton]
public sealed class GlobalChatController
{
	private readonly ILogger _logger;
	private readonly PlayerController _playerController;
	private readonly ChatActor _chatActor;

	public GlobalChatController(ILogger l, PlayerController pc, ChatEvents ce, ChatActor ca)
	{
		_logger = l.ForThisContext();
		_playerController = pc;
		_chatActor = ca;
		ce.OnChatMessage += OnChatMessage;
	}

	/// <summary>
	/// Handles incoming chat messages from players and broadcasts them to all players in the global chat channel.
	/// </summary>
	/// <param name="player">The player who sent the chat message.</param>
	/// <param name="message">The chat message sent by the player.</param>
	/// <remarks>
	/// This method checks if the message is not a command (does not start with '/') and if the player is in the global chat channel.
	/// If both conditions are met, the message is broadcast to all players in the global chat channel, prefixed with the sender's username.
	/// </remarks>
	private async Task OnChatMessage(PiPlayer player, string message)
	{
		try
		{
			if (message.StartsWith('/') || player.ActiveChatChannel != CHATCHANNEL.GLOBAL) return;

			var msg = new ChatMessage.Builder()
				.AddSender(player.Username)
				.AddText(message)
				.Build().Message;

			foreach (var targetPlayer in _playerController.Players.Values)
				_chatActor.SendMessageToPlayer(targetPlayer, msg);
		}
		catch (Exception ex)
		{
			_logger.Error(ex);
		}
	}

	/// <summary>
	/// Handles the "/global" slash command, which switches the player's active chat channel to the global channel.
	/// </summary>
	/// <param name="player">The player who executed the command.</param>
	/// <param name="_"></param>
	[SlashCommand("global")]
	public async Task GlobalCommand(PiPlayer player, string _)
	{
		player.ActiveChatChannel = CHATCHANNEL.GLOBAL;
		_chatActor.SendMessageToPlayer(player, $"You are now talking in {player.ActiveChatChannel}");
	}
}

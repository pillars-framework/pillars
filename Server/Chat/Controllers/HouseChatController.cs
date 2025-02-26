namespace Pillars.Chat.Controllers;

[RegisterSingleton]
public sealed class HouseChatController
{
	private readonly ILogger _logger;
	private readonly PlayerController _playerController;
	private readonly ChatActor _chatActor;

	public HouseChatController(ILogger l, PlayerController pc, ChatEvents ce, ChatActor ca)
	{
		_logger = l.ForThisContext();
		_playerController = pc;
		_chatActor = ca;
		ce.OnChatMessage += OnChatMessage;
	}

	/// <summary>
	/// Handles incoming chat messages from players and broadcasts them to all players within the same house.
	/// </summary>
	/// <param name="player">The player who sent the chat message.</param>
	/// <param name="message">The chat message sent by the player.</param>
	/// <remarks>
	/// This method checks if the message is not a command (does not start with '/') and if the player is in the house chat channel.
	/// If both conditions are met, the message is broadcast to all players who belong to the same house as the sender.
	///
	/// The message is formatted to include the sender's house affiliation and username, wrapped in house-specific tags for display purposes.
	/// </remarks>
	private async Task OnChatMessage(PiPlayer player, string message)
	{
		try
		{
			if (message.StartsWith('/') || player.ActiveChatChannel != CHATCHANNEL.HOUSE) return;

			var msg = new ChatMessage.Builder()
				.AddSender(player.Username, (CHATTEXTSTYLE)player.House)
				.AddIcon((CHATICON)player.House)
				.AddText(message)
				.Build().Message;

			foreach (var targetPlayer in _playerController.Players.Values.Where(
				         p => p.House == player.House))
				_chatActor.SendMessageToPlayer(targetPlayer, msg);
		}
		catch (Exception ex)
		{
			_logger.Error(ex);
		}
	}

	/// <summary>
	/// Handles the "/house" slash command, which switches the player's active chat channel to the house channel.
	/// </summary>
	/// <param name="player">The player who executed the command.</param>
	[SlashCommand("house")]
	private async Task HouseCommand(PiPlayer player)
	{
		player.ActiveChatChannel = CHATCHANNEL.HOUSE;
		_chatActor.SendMessageToPlayer(player, $"You are now talking in {player.ActiveChatChannel}");
	}
}

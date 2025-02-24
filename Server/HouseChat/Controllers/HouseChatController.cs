namespace Pillars.HouseChat.Controllers;

[RegisterSingleton]
public sealed class HouseChatController
{
	private readonly PlayerController _playerController;
	private readonly PiChatActor _piChatActor;

	public HouseChatController( PlayerController pc, ChatEvents ce, PiChatActor pica)
	{
		_playerController = pc;
		_piChatActor = pica;
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
		if (message.StartsWith('/') || player.ActiveChatChannel != CHATCHANNEL.HOUSE) return;

		foreach (var targetPlayer in _playerController.Players.Keys.Where(p => p.House == player.House))
			_piChatActor.SendMessageToPlayer(targetPlayer, "<img id=\"" + (HOUSE)player.House + "\"/><" + (HOUSE)player.House + ">" + player.Username + ": " + message + "</>");
	}

	/// <summary>
	/// Handles the "/house" slash command, which switches the player's active chat channel to the house channel.
	/// </summary>
	/// <param name="player">The player who executed the command.</param>
	[SlashCommand("house")]
	public async Task HouseCommand(PiPlayer player) => player.ActiveChatChannel = CHATCHANNEL.HOUSE;

}

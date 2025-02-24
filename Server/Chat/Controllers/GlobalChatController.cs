﻿namespace Pillars.Chat.Controllers;

[RegisterSingleton]
public sealed class GlobalChatController
{
	private readonly PlayerController _playerController;
	private readonly PiChatActor _piChatActor;

	public GlobalChatController(PlayerController pc, ChatEvents ce, PiChatActor pica)
	{
		_playerController = pc;
		_piChatActor = pica;
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
		if (message.StartsWith('/') || player.ActiveChatChannel != CHATCHANNEL.GLOBAL) return;

		foreach (var targetPlayer in _playerController.Players.Keys)
			_piChatActor.SendMessageToPlayer(targetPlayer, $"{player.Username}: " + message);

	}

	/// <summary>
	/// Handles the "/global" slash command, which switches the player's active chat channel to the global channel.
	/// </summary>
	/// <param name="player">The player who executed the command.</param>
	[SlashCommand("global")]
	public async Task GlobalCommand(PiPlayer player)
	{
		player.ActiveChatChannel = CHATCHANNEL.GLOBAL;
		_piChatActor.SendMessageToPlayer(player.Player, $"You are now talking in {player.ActiveChatChannel}");
	}

}

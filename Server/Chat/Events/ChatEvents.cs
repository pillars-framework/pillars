namespace Pillars.Chat.Events;

[RegisterSingleton]
public sealed class ChatEvents
{
	#region CLIENT MESSAGE

	/// <summary>
	/// Represents a delegate for handling chat message events.
	/// </summary>
	/// <param name="player">The player who sent the chat message.</param>
	/// <param name="message">The chat message sent by the player.</param>
	public delegate Task OnChatMessageDelegate(PiPlayer player, string message);

	/// <summary>
	/// Occurs when a chat message is sent by a player from the client.
	/// </summary>
	public event OnChatMessageDelegate? OnChatMessage;

	/// <summary>
	/// Raises the <see cref="OnChatMessage"/> event.
	/// </summary>
	/// <param name="player">The player who sent the chat message.</param>
	/// <param name="message">The chat message sent by the player.</param>
	/// <remarks>
	/// This method is used to trigger the <see cref="OnChatMessage"/> event, notifying all subscribers of the new chat message.
	/// </remarks>
	public void ChatMessage(PiPlayer player, string message) =>
		OnChatMessage?.Invoke(player, message);

	#endregion
}

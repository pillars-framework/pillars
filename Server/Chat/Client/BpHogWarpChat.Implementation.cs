namespace HogWarp.Replicated;

public sealed partial class BpHogWarpChat
{
	/// <summary>
	/// Gets or sets the parent chat actor associated with this instance.
	/// </summary>
	/// <value>
	/// The <see cref="PiChatActor"/> instance representing the parent chat actor.
	/// </value>
	public required PiChatActor ParentActor { get; set; }

	/// <summary>
	/// Sends a message to the specified player by forwarding it to the parent chat actor.
	/// </summary>
	/// <param name="player">The player to whom the message is sent.</param>
	/// <param name="message">The message to send.</param>
	public partial void SendMsg(NativePlayer player, string message) => ParentActor.PlayerMessageReceived(player, message);
}

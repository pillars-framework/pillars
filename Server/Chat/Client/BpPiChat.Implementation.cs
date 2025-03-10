namespace HogWarp.Replicated;

public sealed partial class BpPiChat
{
	/// <summary>
	/// Gets or sets the parent chat actor associated with this instance.
	/// </summary>
	/// <value>
	/// The <see cref="ChatActor"/> instance representing the parent chat actor.
	/// </value>
	public required ChatActor ParentActor { get; set; }

	/// <summary>
	/// Invoked when a player sends a message to the server
	/// </summary>
	/// <param name="player">The player to who message sent.</param>
	/// <param name="message">The content</param>
	public partial void FromClient(NativePlayer player, string message) =>
		ParentActor.PlayerMessageReceived(player, message);

	/// <summary>
	/// Invoked when a player closes the settings window of the chat panel.
	/// </summary>
	/// <param name="player">The player that closed the window</param>
	/// <param name="height">The height value of the chat panel window</param>
	/// <param name="width">The width value of the chat panel window</param>
	/// <param name="textScale">The font size of the chat panel window</param>
	public partial void SaveSettings(NativePlayer player, float height, float width, float textScale) =>
		ParentActor.SavePlayerSettings(player, height, width, textScale);
}

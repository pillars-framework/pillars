namespace Pillars.Core.Player.Models;

public sealed partial class PiPlayer
{
	/// <summary>
	/// Gets or sets the active chat channel for the player.
	/// </summary>
	/// <remarks>
	/// This property determines the current chat channel in which the player is participating.
	/// The default value is <see cref="CHATCHANNEL.GLOBAL"/>, meaning the player starts in the global chat channel.
	/// </remarks>
	public CHATCHANNEL ActiveChatChannel { get; set; } = CHATCHANNEL.GLOBAL;
}

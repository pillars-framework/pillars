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

	/// <summary>
	/// Flag to indicate if the player has parseltongue enabled
	/// </summary>
	/// <remarks>
	/// All chat messages sent by a player with parseltongue enabled will only
	/// be able to be viewed correctly by all other players with the ability to speak parseltongue.
	/// </remarks>
	public bool IsParselEnabled { get; set; }

	/// <summary>
	/// Internal flag
	/// </summary>
	private bool _canSpeakParsel;

	/// <summary>
	/// Flag to indicate if the player has the ability to speak parseltongue
	/// </summary>
	/// <remarks>
	/// All chat messages sent by a player with parseltongue enabled will only
	/// be able to be viewed correctly by all other players with the ability to speak parseltongue.
	/// If disabled, also disables any active parseltongue.
	/// </remarks>
	public bool CanSpeakParsel
	{
		get => _canSpeakParsel;
		set
		{
			_canSpeakParsel = value;
			if (!_canSpeakParsel)
				IsParselEnabled = false;
		}
	}
}

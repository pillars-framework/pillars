namespace Pillars.Core.Player.Models;

[RegisterTransient]
public sealed partial class PiPlayer(NativePlayer native, Account acc)
{
	/// <summary>
	/// Flag to indicate if the player object is valid / invalid
	/// </summary>
	public bool IsValid => Id != 0xE000000F;
}

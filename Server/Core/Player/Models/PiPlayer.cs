namespace Pillars.Core.Player.Models;

public sealed partial class PiPlayer(NativePlayer native, Account acc)
{
	public NativePlayer Native { get; set; } = native;
}

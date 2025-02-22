namespace Pillars.Core.Player.Models;

public sealed partial class PiPlayer(HPlayer player, Account acc)
{
	public HPlayer Player { get; set; } = player;
}

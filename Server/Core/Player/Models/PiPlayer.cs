namespace Pillars.Core.Player.Models;

public sealed partial class PiPlayer(HPlayer player)
{
    public HPlayer Player { get; set; } = player;

    public ulong DiscordId
    {
        get {
            var match = Regex.Match(Player.UniqueId, @"discord:(\d+)");
            return match.Success ? ulong.Parse(match.Groups[1].Value) : 0;
        }
    }


}

namespace Pillars.Core.Player.Helpers;

public static partial class PlayerHelper
{
	#region DISCORD ID

	/// <summary>
	/// For a given Hogwarp Player, returns the parsed discordId (stripping the 'discord:').
	/// Returns 0 if no discordId is found.
	/// </summary>
	public static ulong GetDiscordId(HPlayer player)
	{
		var match = MyRegex().Match(player.UniqueId);
		return match.Success ? ulong.Parse(match.Groups[1].Value) : 0;
	}

	/// <summary>
	/// For a given PiPlayer, returns the parsed discordId (stripping the 'discord:').
	/// Returns 0 if no discordId is found.
	/// </summary>
	public static ulong GetDiscordId(PiPlayer player)
	{
		var match = MyRegex().Match(player.UniqueId);
		return match.Success ? ulong.Parse(match.Groups[1].Value) : 0;
	}

	[GeneratedRegex(@"discord:(\d+)")]
	private static partial Regex MyRegex();

	#endregion
}

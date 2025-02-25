namespace Pillars.Core.Player.Extensions;

/// <summary>
/// Extension methods for the native (HogWarp) player
/// </summary>
public static partial class NativePlayerExtensions
{
	/// <summary>
	/// Checks if the given player is valid by comparising to the disposed Id <c>0xDFDFDFDF</c>
	/// </summary>
	/// <param name="nativePlayer">The player to check if valid</param>
	/// <returns>
	/// <c>true</c>- if the player is valid <br/>
	/// <c>false</c>- if the player is invalid
	/// </returns>
	public static bool IsValid(this NativePlayer nativePlayer) =>
		nativePlayer.Id != 0xDFDFDFDF;

	/// <summary>
	/// Strips the prefix of the discordId and returns it as a ulong
	/// </summary>
	/// <param name="nativePlayer">The native player to get the discordId from</param>
	/// <returns>
	/// <c>discordId</c>- if the player is valid and discordId is found <br/>
	/// <c>null</c>- if the player is invalid or if not found
	/// </returns>
	public static ulong? GetDiscordId(this NativePlayer nativePlayer)
	{
		if (!nativePlayer.IsValid())
			return null;
		var match = DiscordIdRegex().Match(nativePlayer.UniqueId);
		return match.Success ? ulong.Parse(match.Groups[1].Value) : null;
	}

	[GeneratedRegex(@"discord:(\d+)")]
	private static partial Regex DiscordIdRegex();
}

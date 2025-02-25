namespace Pillars.Core.Player.Models;

[RegisterTransient]
public sealed partial class PiPlayer(ILogger l, NativePlayer native, Account acc)
{
	private readonly ILogger _logger = l.ForThisContext();

	/// <summary>
	/// Flag to indicate if the player object is valid / invalid
	/// </summary>
	public bool IsValid => Native.IsValid();

	/// <summary>
	/// Returns a possible discordId for the player
	/// </summary>
	/// <returns>
	/// <c>discordId</c>- if the player is valid and discordId is found <br/>
	/// <c>null</c>- if the player is invalid or if not found
	/// </returns>
	public ulong? DiscordId => Native.GetDiscordId();
}

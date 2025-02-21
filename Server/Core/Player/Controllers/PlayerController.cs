namespace Pillars.Core.Player.Controllers;

[RegisterSingleton]
public sealed class PlayerController(ILogger l)
{
	private readonly ILogger _logger = l.ForThisContext();

	/// <summary>
	/// Maps HPlayer to PiPlayer
	/// </summary>
	public readonly ConcurrentDictionary<HPlayer, PiPlayer> Players = [];

	/// <summary>
	/// For a given HogWarp Player, returns the corresponding PiPlayer
	/// </summary>
	public PiPlayer? GetPlayer(HPlayer hplayer) =>
		Players.TryGetValue(hplayer, out var player) ? player : null;

	/// <summary>
	/// For a given playerId, returns a possible corresponding PiPlayer
	/// </summary>
	public PiPlayer? GetPlayer(ulong playerId)
	{
		var p = HogWarpSdk.Server.PlayerSystem.GetById(playerId);
		return Players.TryGetValue(p, out var player) ? player : null;
	}

	/// <summary>
	/// For a given HogWarp Player, returns a newly created PiPlayer.
	/// Prioritizes existing player over creating new.
	/// </summary>
	public PiPlayer CreatePlayer(HPlayer hplayer)
	{
		if (Players.TryGetValue(hplayer, out var player))
			return player;
		var newPlayer = new PiPlayer(hplayer);
		Players.TryAdd(hplayer, newPlayer);
		return newPlayer;
	}

	/// <summary>
	/// Destroys the given player
	/// </summary>
	public PiPlayer? DestroyPlayer(HPlayer hplayer) =>
		Players.TryRemove(hplayer, out var player) ? player : null;

	/// <summary>
	/// Detroys a player by a given playerId
	/// </summary>
	public PiPlayer? DestroyPlayer(ulong playerId)
	{
		var player = HogWarpSdk.Server.PlayerSystem.GetById(playerId);
		// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
		return player is null ? null : DestroyPlayer(player);
	}
}

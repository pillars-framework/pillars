namespace Pillars.Core.Player.Controllers;

/// <summary>
/// Handles HogWarp player join / leave events
/// </summary>
[RegisterSingleton]
public sealed class PlayerController
{
	private readonly ILogger _logger;
	private readonly AccountController _accountController;
	private readonly PlayerConnectionEvents _playerConnectionEvents;

	public PlayerController(ILogger l, PlayerConnectionEvents pce, AccountController ac)
	{
		_logger = l.ForThisContext();
		_playerConnectionEvents = pce;
		_accountController = ac;
		HogWarpSdk.Server.PlayerSystem.PlayerJoinEvent += async p => PlayerJoined(p);
		HogWarpSdk.Server.PlayerSystem.PlayerLeftEvent += async p => PlayerLeft(p);
	}

	#region COLLECTION

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
	/// Returns an existing PiPlayer if one already exists
	/// </summary>
	private PiPlayer CreatePlayer(HPlayer hplayer, Account acc)
	{
		if (Players.TryGetValue(hplayer, out var player))
			return player;
		var newPlayer = new PiPlayer(hplayer, acc);
		Players.TryAdd(hplayer, newPlayer);
		return newPlayer;
	}

	/// <summary>
	/// Destroys the given player
	/// </summary>
	private PiPlayer? DestroyPlayer(HPlayer hplayer) =>
		Players.TryRemove(hplayer, out var player) ? player : null;

	/// <summary>
	/// Detroys a player by a given playerId
	/// </summary>
	private PiPlayer? DestroyPlayer(ulong playerId)
	{
		var player = HogWarpSdk.Server.PlayerSystem.GetById(playerId);
		// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
		return player is null ? null : DestroyPlayer(player);
	}

	#endregion

	#region CONNECT / DISCONNECT

	/// <summary>
	/// If a player joins the server, fetches or creates a corresponding account.
	/// Creates a PiPlayer with corresponding account attached and fires the
	/// <see cref="PlayerConnectionEvents.OnPlayerConnected"/>
	/// </summary>
	/// <param name="hplayer">The HogWarp Player joining</param>
	private async Task PlayerJoined(HPlayer hplayer)
	{
		try
		{
			// Retrieve discord id or kick if unsuccessful
			var discordId = PlayerHelper.GetDiscordId(hplayer);
			if (discordId == 0)
			{
				_logger.Error(
					"Failed to determine discord id for player from UniqueId. Id - {id}, ConnectionId - {cid}, UniqueId - {uid} ",
					hplayer.Id, hplayer.ConnectionId, hplayer.UniqueId);
				hplayer.Kick();
				return;
			}

			// Create / retrieve account or kick if unsuccessful
			var acc = await _accountController.GetOrCreateAccountAsync(discordId);
			if (acc is null)
			{
				_logger.Error(
					"Failed to create or retrieve Account for player. Id - {id} - ConnectionId - {cid}, UniqueId - {uid} ",
					hplayer.Id, hplayer.ConnectionId, hplayer.UniqueId);
				hplayer.Kick();
				return;
			}

			// Create player and dispatch event
			var player = CreatePlayer(hplayer, acc);
			_logger.Information("New player connect. AccountId - {aid}, DiscordId - {did}",
				player.Account.ID, discordId);
			_playerConnectionEvents.PlayerConnected(player);
		}
		catch (Exception ex)
		{
			_logger.Error(ex);
		}
	}

	private async Task PlayerLeft(HPlayer hplayer)
	{
		try
		{
		}
		catch (Exception ex)
		{
			_logger.Error(ex);
		}
	}

	#endregion
}

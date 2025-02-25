namespace Pillars.Core.Player.Controllers;

/// <summary>
/// Handles HogWarp player join / leave events
/// </summary>
[RegisterSingleton]
public sealed class PlayerController
{
	private readonly ILogger _logger;
	private readonly PlayerFactory _playerFactory;
	private readonly AccountController _accountController;
	private readonly PlayerConnectionEvents _playerConnectionEvents;

	public PlayerController(ILogger l, PlayerFactory pf, PlayerConnectionEvents pce, AccountController ac)
	{
		_logger = l.ForThisContext();
		_playerFactory = pf;
		_playerConnectionEvents = pce;
		_accountController = ac;
		HogWarpSdk.Server.PlayerSystem.PlayerJoinEvent += async p => PlayerJoined(p);
		HogWarpSdk.Server.PlayerSystem.PlayerLeftEvent += async p => PlayerLeft(p);
	}

	#region COLLECTION

	/// <summary>
	/// Maps NativePlayer to PiPlayer
	/// </summary>
	public readonly ConcurrentDictionary<NativePlayer, PiPlayer> Players = [];

	/// <summary>
	/// For a given HogWarp Player, returns the corresponding PiPlayer
	/// </summary>
	public PiPlayer? GetPlayer(NativePlayer native) =>
		Players.TryGetValue(native, out var player) ? player : null;

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
	private PiPlayer CreatePlayer(NativePlayer native, Account acc)
	{
		if (Players.TryGetValue(native, out var player))
			return player;
		player = _playerFactory.Create(native, acc);
		Players.TryAdd(native, player);
		return player;
	}

	/// <summary>
	/// Destroys the given player
	/// </summary>
	private PiPlayer? DestroyPlayer(NativePlayer native) =>
		Players.TryRemove(native, out var player) ? player : null;

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
	/// <param name="native">The HogWarp Player joining</param>
	private async Task PlayerJoined(NativePlayer native)
	{
		try
		{
			if (!native.IsValid())
			{
				_logger.Warning("{m} - Player is not valid anymore, aborting connection.",
					nameof(PlayerJoined));
				return;
			}

			// Retrieve discord id or kick if unsuccessful
			var discordId = native.GetDiscordId();
			if (discordId is null)
			{
				_logger.Error(
					"Failed to determine discord id for player from UniqueId. Id - {id}, ConnectionId - {cid} ",
					native.Id, native.ConnectionId);
				if (native.IsValid())
					native.Kick();
				return;
			}

			// Create / retrieve account or kick if unsuccessful
			var acc = await _accountController.GetOrCreateAccountAsync((ulong)discordId);
			if (acc is null)
			{
				_logger.Error(
					"Failed to create or retrieve Account for player. Id - {id} - ConnectionId - {cid} ",
					native.Id, native.ConnectionId);
				if (native.IsValid())
					native.Kick();
				return;
			}

			// Create player and dispatch event
			var player = CreatePlayer(native, acc);
			_logger.Information("New player connect. AccountId - {aid}, DiscordId - {did}",
				player.Account.ID, discordId);
			_playerConnectionEvents.PlayerConnected(player);
		}
		catch (Exception ex)
		{
			_logger.Error(ex);
		}
	}

	/// <summary>
	/// If a player leaves the server, we check if the player was correctly loaded /
	/// a PiPlayer exists. If so, we will invoke an event and clear the collection
	/// <see cref="PlayerConnectionEvents.OnPlayerDisconnected"/>
	/// </summary>
	/// <param name="native">The HogWarp Player leaving</param>
	private async Task PlayerLeft(NativePlayer native)
	{
		try
		{
			var piPlayer = DestroyPlayer(native);
			if (piPlayer is null)
				return;
			_playerConnectionEvents.PlayerDisconnected(piPlayer);
		}
		catch (Exception ex)
		{
			_logger.Error(ex);
		}
	}

	#endregion
}

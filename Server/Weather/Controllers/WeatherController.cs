namespace Pillars.Weather.Controllers;

[RegisterSingleton]
public sealed class WeatherController
{
	private readonly ILogger _logger;
	private readonly WeatherActor _weatherActor;
	private readonly PlayerController _playerController;
	private SEASON _currentSeason = SEASON.Winter;
	private WEATHER _currentWeather = WEATHER.Clear;
	private DateTime _currentTime = DateTime.Now;

	public WeatherController(ILogger l, WeatherEvents we, WeatherActor wa, PlayerConnectionEvents pce, PlayerController pc)
	{
		_logger = l.ForThisContext();
		_weatherActor = wa;
		_playerController = pc;
		pce.OnPlayerConnected += OnPlayerConnected;
		we.OnWeatherChange += OnWeatherChange;
		we.OnSeasonChange += OnSeasonChange;
		we.OnTimeChange += OnTimeChange;
	}

	#region EVENTS
	/// <summary>
	/// Handles the event when a player connects to the server.
	/// </summary>
	/// <remarks>
	/// This method synchronizes the world state for the newly connected player by calling <see cref="SyncWorld"/>.
	/// </remarks>
	/// <param name="player">The player who connected to the server.</param>
	private async Task OnPlayerConnected(PiPlayer player)
		=> SyncWorld(player);

	/// <summary>
	/// Handles the event when the weather changes in the game.
	/// </summary>
	/// <remarks>
	/// This method updates the server's weather state by calling <see cref="SetServerWeather"/>
	/// with the provided <paramref name="weather"/> value.
	/// </remarks>
	/// <param name="weather">The new weather to apply to the server.</param>
	private async Task OnWeatherChange(WEATHER weather)
		=> SetServerWeather(weather);

	/// <summary>
	/// Handles the event when the season changes in the game.
	/// </summary>
	/// <remarks>
	/// This method updates the server's season state by calling <see cref="SetServerSeason"/>
	/// with the provided <paramref name="season"/> value.
	/// </remarks>
	/// <param name="season">The new season to apply to the server.</param>
	private async Task OnSeasonChange(SEASON season)
		=> SetServerSeason(season);

	/// <summary>
	/// Handles the event when the in-game time changes.
	/// </summary>
	/// <remarks>
	/// This method updates the server's time state by calling <see cref="SetServerTime"/>
	/// with the provided <paramref name="datetime"/> value.
	/// </remarks>
	/// <param name="datetime">The new date and time to apply to the server.</param>
	private async Task OnTimeChange(DateTime datetime)
		=> SetServerTime(datetime);
	#endregion


	#region METHODS

	/// <summary>
	/// Synchronizes the world state for a specific player by updating their season, weather, and time.
	/// </summary>
	/// <remarks>
	/// This method sets the player's current season, weather, and time to match the server's state.
	/// It calls the following methods on the <see cref="_weatherActor"/>:
	/// </remarks>
	/// <param name="player">The player whose world state will be synchronized.</param>
	private void SyncWorld(PiPlayer player)
	{
		_weatherActor.SetPlayerSeason(player, _currentSeason);
		_weatherActor.SetPlayerWeather(player, _currentWeather);
		_weatherActor.SetPlayerTime(player, _currentTime);
	}

	/// <summary>
	/// Updates the server's weather state and applies it to all connected players.
	/// </summary>
	/// <remarks>
	/// This method sets the server's current weather to the specified <paramref name="weather"/> value
	/// and updates the weather for all connected players.
	/// </remarks>
	/// <param name="weather">The new weather to apply to the server and all players.</param>
	public void SetServerWeather(WEATHER weather)
	{
		_currentWeather = weather;
		foreach (var player in _playerController.Players.Values)
			_weatherActor.SetPlayerWeather(player, weather);

		_logger.Information("Weather was set to: {w}", _currentWeather);
	}

	/// <summary>
	/// Updates the server's season state and applies it to all connected players.
	/// </summary>
	/// <remarks>
	/// This method sets the server's current season to the specified <paramref name="season"/> value
	/// and updates the season for all connected players.
	/// </remarks>
	/// <param name="season">The new season to apply to the server and all players.</param>
	public void SetServerSeason(SEASON season)
	{
		_currentSeason = season;
		foreach (var player in _playerController.Players.Values)
			_weatherActor.SetPlayerSeason(player, season);

		_logger.Information("Season was set to: {s}", _currentSeason);
	}

	/// <summary>
	/// Updates the server's time state and applies it to all connected players.
	/// </summary>
	/// <remarks>
	/// This method sets the server's current time to the specified <paramref name="dateTime"/> value
	/// and updates the time for all connected players.
	/// </remarks>
	/// <param name="dateTime">The new date and time to apply to the server and all players.</param>
	public void SetServerTime(DateTime dateTime)
	{
		_currentTime = dateTime;
		foreach (var player in _playerController.Players.Values)
			_weatherActor.SetPlayerTime(player, dateTime);

		_logger.Information("Time was set to: {t}", _currentTime);
	}
	#endregion

	#region CYCLES
	/// <summary>
	/// Executes a weather cycle, updating the in-game time and synchronizing the world state for all players.
	/// </summary>
	/// <remarks>
	/// This method is part of a cycle system, as indicated by the <see cref="CycleAttribute"/>.
	/// It increments the current time by 1 minute and synchronizes the world state for all connected players
	/// by calling <see cref="SyncWorld"/> for each player. This ensures that all players are updated with the latest
	/// time and weather conditions.
	/// </remarks>
	[Cycle(CYCLE.WEATHER, 1, false, CATCHUP.NONE)]
	public async Task<bool> WeatherCycle()
	{
		_currentTime = _currentTime.AddMinutes(1);
		foreach (var player in _playerController.Players.Values)
			SyncWorld(player);

		return true;
	}
	#endregion

	#region COMMANDS
	/// <summary>
	/// Handles the "/weather" slash command to set the server's weather.
	/// </summary>
	/// <remarks>
	/// This command allows a player to change the server's weather. It expects a single argument,
	/// which is parsed into a <see cref="WEATHER"/> enum value. If the parsing is successful,
	/// the server's weather is updated using <see cref="SetServerWeather"/>, and the change is logged.
	/// </remarks>
	/// <param name="player">The player who issued the command.</param>
	/// <param name="args">The arguments provided with the command. The first argument should be a valid weather id.</param>
	[SlashCommand("weather")]
	public async Task SetWeatherCommand(PiPlayer player, string[] args)
	{
		if(args.Length < 1) return;
		if (!Enum.TryParse(args[0], out WEATHER weather))return;

		SetServerWeather(weather);
		_logger.Information("Weather was set to: {w}", weather);
	}

	/// <summary>
	/// Handles the "/season" slash command to set the server's season.
	/// </summary>
	/// <remarks>
	/// This command allows a player to change the server's season. It expects a single argument,
	/// which is parsed into a <see cref="SEASON"/> enum value. If the parsing is successful,
	/// the server's season is updated using <see cref="SetServerSeason"/>, and the change is logged.
	/// </remarks>
	/// <param name="player">The player who issued the command.</param>
	/// <param name="args">The arguments provided with the command. The first argument should be a valid season id.</param>
	[SlashCommand("season")]
	public async Task SetSeasonCommand(PiPlayer player, string[] args)
	{
		if(args.Length < 1) return;
		if (!Enum.TryParse(args[0], out SEASON season))return;

		SetServerSeason(season);
		_logger.Information("Season was set to: {w}", season);
	}

	/// <summary>
	/// Handles the "/time" slash command to set the server's time.
	/// </summary>
	/// <remarks>
	/// This command allows a player to change the server's time. It expects three arguments:
	/// hour, minute, and second. These values are parsed into integers and used to construct
	/// a <see cref="DateTime"/> object. If all arguments are valid, the server's time is updated
	/// using <see cref="SetServerTime"/>, and the change is logged.
	/// </remarks>
	/// <param name="player">The player who issued the command.</param>
	/// <param name="args">The arguments provided with the command. Requires three arguments: hour, minute, and second.</param>
	[SlashCommand("time")]
	public async Task SetTimeCommand(PiPlayer player, string[] args)
	{
		if(args.Length < 3) return;

		if (!int.TryParse(args[0], out int hour))return;
		if (!int.TryParse(args[1], out int minute))return;
		if (!int.TryParse(args[2], out int second))return;

		var dateTime = new DateTime(
			DateTime.Now.Year,
			DateTime.Now.Month,
			DateTime.Now.Day,
			hour,
			minute,
			second
		);

		SetServerTime(dateTime);
		_logger.Information("Time was set to: {w}", dateTime);
	}
	#endregion
}

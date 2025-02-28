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
	private async Task OnPlayerConnected(PiPlayer player)
		=> SyncWorld(player);

	private async Task OnWeatherChange(WEATHER weather)
		=> SetServerWeather(weather);

	private async Task OnSeasonChange(SEASON season)
		=> SetServerSeason(season);

	private async Task OnTimeChange(DateTime datetime)
		=> SetServerTime(datetime);
	#endregion


	#region METHODS

	private void SyncWorld(PiPlayer player)
	{
		_weatherActor.SetPlayerSeason(player, _currentSeason);
		_weatherActor.SetPlayerWeather(player, _currentWeather);
		_weatherActor.SetPlayerTime(player, _currentTime);
	}

	public void SetServerWeather(WEATHER weather)
	{
		_currentWeather = weather;
		foreach (var player in _playerController.Players.Values)
			_weatherActor.SetPlayerWeather(player, weather);

		_logger.Information("Weather was set to: {w}", _currentWeather);
	}

	public void SetServerSeason(SEASON season)
	{
		_currentSeason = season;
		foreach (var player in _playerController.Players.Values)
			_weatherActor.SetPlayerSeason(player, season);

		_logger.Information("Season was set to: {s}", _currentSeason);
	}

	public void SetServerTime(DateTime dateTime)
	{
		_currentTime = dateTime;
		foreach (var player in _playerController.Players.Values)
			_weatherActor.SetPlayerTime(player, dateTime);

		_logger.Information("Time was set to: {t}", _currentTime);
	}
	#endregion

	#region CYCLES
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

	[SlashCommand("weather")]
	public async Task SetWeatherCommand(PiPlayer player, string[] args)
	{
		if(args.Length < 1) return;
		if (!Enum.TryParse(args[0], out WEATHER weather))return;

		SetServerWeather(weather);
		_logger.Information("Weather was set to: {w}", weather);
	}

	[SlashCommand("season")]
	public async Task SetSeasonCommand(PiPlayer player, string[] args)
	{
		if(args.Length < 1) return;
		if (!Enum.TryParse(args[0], out SEASON season))return;

		SetServerSeason(season);
		_logger.Information("Season was set to: {w}", season);
	}

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

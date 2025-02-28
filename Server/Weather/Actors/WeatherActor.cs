namespace Pillars.Weather.Actors;

[RegisterSingleton]
public sealed class WeatherActor : PiActor<BpHogWarpWeather>
{
	private readonly ILogger _logger;
	private readonly WeatherEvents _weatherEvents;

	public WeatherActor(ILogger l, WeatherEvents we)
	{
		_logger = l;
		_weatherEvents = we;
		_worldActor.ParentActor = this;
	}

	public void SetPlayerWeather(PiPlayer player, WEATHER weather)
		=> _worldActor.UpdateWeather(player.Native, weather.ToString());

	public void SetPlayerSeason(PiPlayer player, SEASON season)
		=> _worldActor.UpdateSeason(player.Native, (int)season);

	public void SetPlayerTime(PiPlayer player, DateTime dateTime)
		=> _worldActor.UpdateTime(player.Native, dateTime.Hour, dateTime.Minute, dateTime.Second);

	public void WeatherChanged(string weather)
	{
		if (Enum.TryParse(weather, true, out WEATHER weatherValue))
			_weatherEvents.WeatherChange(weatherValue);
		else
		{
			_logger.Information("Could not cast weather {w} to enum value, applying clear weather instead.", weather);
			_weatherEvents.WeatherChange(WEATHER.Clear);
		}
	}

	public void SeasonChanged(int season)
	{
		if (Enum.IsDefined(typeof(SEASON), season))
			_weatherEvents.SeasonChange((SEASON) season);
		else
		{
			_logger.Information("Could not cast season {s} to enum value, applying season fall instead.", season);
			_weatherEvents.SeasonChange(SEASON.Fall);
		}
	}

	public void TimeChanged(int hour, int minute, int second)
	{
		var dateTime = new DateTime(
			DateTime.Now.Year,
			DateTime.Now.Month,
			DateTime.Now.Day,
			hour,
			minute,
			second
		);
		_weatherEvents.TimeChange(dateTime);
	}

}

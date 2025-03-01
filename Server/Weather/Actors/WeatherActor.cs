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

	/// <summary>
	/// Updates the weather for a specific player in the game world.
	/// </summary>
	/// <param name="player">The player whose weather will be updated.</param>
	/// <param name="weather">The weather to apply to the player.</param>
	public void SetPlayerWeather(PiPlayer player, WEATHER weather)
		=> _worldActor.UpdateWeather(player.Native, weather.ToString());

	/// <summary>
	/// Updates the season for a specific player in the game world.
	/// </summary>
	/// <param name="player">The player whose season will be updated.</param>
	/// <param name="season">The season to apply to the player.</param>
	public void SetPlayerSeason(PiPlayer player, SEASON season)
		=> _worldActor.UpdateSeason(player.Native, (int)season);

	/// <summary>
	/// Updates the in-game time for a specific player.
	/// </summary>
	/// <param name="player">The player whose time will be updated.</param>
	/// <param name="dateTime">The <see cref="DateTime"/> object containing the new time values.</param>
	public void SetPlayerTime(PiPlayer player, DateTime dateTime)
		=> _worldActor.UpdateTime(player.Native, dateTime.Hour, dateTime.Minute, dateTime.Second);

	/// <summary>
	/// Triggers the WeatherChange event with the provided weather value
	/// </summary>
	/// <remarks>
	/// This method attempts to parse the provided <paramref name="weather"/> string into a valid <see cref="WEATHER"/> enum value.
	/// If the parsing is successful, it triggers a weather change event with the provided value.
	/// If the parsing fails, it logs a warning message using <see cref="_logger"/> and defaults to applying the <see cref="WEATHER.Clear"/> weather.
	/// </remarks>
	/// <param name="weather">The string representation of the weather to be applied.</param>
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

	/// <summary>
	/// Triggers the SeasonChange event with the provided season value
	/// </summary>
	/// <remarks>
	/// This method checks if the provided <paramref name="season"/> value corresponds to a valid <see cref="SEASON"/> enum.
	/// If the value is valid, it triggers a season change event with the provided value.
	/// If the value is invalid, it logs a warning message using <see cref="_logger"/> and defaults to applying the <see cref="SEASON.Fall"/> season.
	/// </remarks>
	/// <param name="season">The integer value representing the season to be applied.</param>
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

	/// <summary>
	/// Triggers the TimeChange event with the provided hour, minute, and second values.
	/// </summary>
	/// <remarks>
	/// This method constructs a <see cref="DateTime"/> object using the current year, month, and day,
	/// combined with the specified <paramref name="hour"/>, <paramref name="minute"/>, and <paramref name="second"/>.
	/// </remarks>
	/// <param name="hour">The hour component of the new time (0-23).</param>
	/// <param name="minute">The minute component of the new time (0-59).</param>
	/// <param name="second">The second component of the new time (0-59).</param>
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

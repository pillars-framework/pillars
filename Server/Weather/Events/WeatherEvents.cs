namespace Pillars.Weather.Events;

[RegisterSingleton]
public sealed class WeatherEvents
{
	/// <summary>
	/// Represents a delegate for handling weather change events.
	/// </summary>
	/// <param name="weather">The new weather condition to be handled.</param>
	public delegate Task OnWeatherChangeDelegate(WEATHER weather);

	/// <summary>
	/// Occurs when a weather change is sent by a player from the client.
	/// </summary>
	public event OnWeatherChangeDelegate? OnWeatherChange;

	/// <summary>
	/// Triggers the <see cref="OnWeatherChange"/> event with the specified weather.
	/// </summary>
	/// <param name="weather">The new weather condition to notify subscribers about.</param>
	public void WeatherChange(WEATHER weather)
		=> OnWeatherChange?.Invoke(weather);

	/// <summary>
	/// Represents a delegate for handling season change events.
	/// </summary>
	/// <param name="season">The new season to be handled.</param>
	public delegate Task OnSeasonChangeDelegate(SEASON season);

	/// <summary>
	///  Occurs when a season change is sent by a player from the client.
	/// </summary>
	public event OnSeasonChangeDelegate? OnSeasonChange;

	/// <summary>
	/// Triggers the <see cref="OnSeasonChange"/> event with the specified season.
	/// </summary>
	/// <param name="season">The new season to notify subscribers about.</param>
	public void SeasonChange(SEASON season)
		=> OnSeasonChange?.Invoke(season);

	/// <summary>
	/// Represents a delegate for handling time change events.
	/// </summary>
	/// <param name="dateTime">The new date and time to be handled.</param>
	public delegate Task OnTimeChangeDelegate(DateTime dateTime);

	/// <summary>
	///  Occurs when a time change is sent by a player from the client.
	/// </summary>
	public event OnTimeChangeDelegate? OnTimeChange;

	/// <summary>
	/// Triggers the <see cref="OnTimeChange"/> event with the specified date and time.
	/// </summary>
	/// <param name="dateTime">The new date and time to notify subscribers about.</param>
	public void TimeChange(DateTime dateTime)
		=> OnTimeChange?.Invoke(dateTime);
}

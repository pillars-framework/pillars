namespace Pillars.Weather.Events;

[RegisterSingleton]
public sealed class WeatherEvents
{
	public delegate Task OnWeatherChangeDelegate(WEATHER weather);

	public event OnWeatherChangeDelegate? OnWeatherChange;

	public void WeatherChange(WEATHER weather)
		=> OnWeatherChange?.Invoke(weather);

	public delegate Task OnSeasonChangeDelegate(SEASON season);

	public event OnSeasonChangeDelegate? OnSeasonChange;

	public void SeasonChange(SEASON season)
		=> OnSeasonChange?.Invoke(season);

	public delegate Task OnTimeChangeDelegate(DateTime dateTime);

	public event OnTimeChangeDelegate? OnTimeChange;

	public void TimeChange(DateTime dateTime)
		=> OnTimeChange?.Invoke(dateTime);
}

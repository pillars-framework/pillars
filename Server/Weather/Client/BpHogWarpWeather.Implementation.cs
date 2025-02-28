namespace HogWarp.Replicated;

public sealed partial class BpHogWarpWeather
{
	public required WeatherActor ParentActor { get; set; }

	public partial void SendWeather(Player player, string weather)
		=> ParentActor.WeatherChanged(weather);

	public partial void SendSeason(Player player, int season)
		=> ParentActor.SeasonChanged(season);

	public partial void SendTime(Player player, int hour, int minute, int second)
		=> ParentActor.TimeChanged(hour, minute, second);
}

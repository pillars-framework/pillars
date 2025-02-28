namespace HogWarp.Replicated;

public sealed partial class BpHogWarpWeather
{
	/// <summary>
	/// Gets or sets the parent chat actor associated with this instance.
	/// </summary>
	/// <value>
	/// The <see cref="WeatherActor"/> instance representing the parent weather actor.
	/// </value>
	public required WeatherActor ParentActor { get; set; }

	/// <summary>
	/// Sends a weather update to the specified player.
	/// </summary>
	/// <param name="player">The player to whom the weather update is sent.</param>
	/// <param name="weather">The string representation of the weather to apply.</param>
	public partial void SendWeather(Player player, string weather)
		=> ParentActor.WeatherChanged(weather);

	/// <summary>
	/// Sends a season update to the specified player.
	/// </summary>
	/// <param name="player">The player to whom the season update is sent.</param>
	/// <param name="season">The integer value representing the season to apply.</param>
	public partial void SendSeason(Player player, int season)
		=> ParentActor.SeasonChanged(season);

	/// <summary>
	/// Sends a time update to the specified player.
	/// </summary>
	/// <param name="player">The player to whom the time update is sent.</param>
	/// <param name="hour">The hour component of the new time (0-23).</param>
	/// <param name="minute">The minute component of the new time (0-59).</param>
	/// <param name="second">The second component of the new time (0-59).</param>
	public partial void SendTime(Player player, int hour, int minute, int second)
		=> ParentActor.TimeChanged(hour, minute, second);
}

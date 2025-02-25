namespace Pillars.Schedules.Helpers;

/// <summary>
/// Helper class for the schedule covering Cron Expressions etc.
/// </summary>
public static class ScheduleHelper
{
	/// <summary>
	/// For a given cronexpression, checks if a "next occurence" exists.
	/// If not, the expression is invalid
	/// </summary>
	public static bool IsValidCronExpression(CronExpression cronExp) =>
		cronExp.GetNextOccurrence(DateTimeOffset.Now, TimeZoneInfo.Local) != null;

	/// <summary>
	/// Calculates the number of occurences that should have happened between
	/// the since and now based on the cron expression.
	///
	/// [IMPORTANT] Excludes the boundaries !
	/// </summary>
	public static int OccurrencesSince(CronExpression cronExp, DateTime since) =>
		cronExp.GetOccurrences(since, DateTimeOffset.Now, TimeZoneInfo.Local, false, false).Count();

	/// <summary>
	/// For a given cycle, calculates how many cycles have been missed,
	/// including the tracker in its calulations, based on the current time.
	/// </summary>
	public static int GetMissedCycleTriggers(Cycle cycle)
	{
		if (!cycle.HasHistory || cycle.LastTrigger == null) return 0;
		// Calculates minutes since history
		var msSinceHistory = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - ((DateTimeOffset)cycle.LastTrigger.Value).ToUnixTimeMilliseconds();
		var minutesSinceHistory = Math.Floor((double)(msSinceHistory / (60 * 1000)));
		// Adjust for already elapsed
		minutesSinceHistory -= cycle.CycleTracker.ElapsedMinutes;
		return (int)Math.Floor(minutesSinceHistory / cycle.Info.CycleTime);
	}
}

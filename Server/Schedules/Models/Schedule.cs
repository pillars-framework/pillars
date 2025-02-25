namespace Pillars.Schedules.Models;

using Timer = System.Timers.Timer;

/// <summary>
/// A Schedule describes a function to be ran at a specific time (e.g. always at 2 pm)
/// Each schedule creates their own timer, so consider a Cycle, if possible.
/// </summary>
/// <param name="scheduleInfo"></param>
/// <param name="cronExp">The parsed cron expression</param>
/// <param name="lastTrigger"></param>
public sealed class Schedule(ScheduleAttribute scheduleInfo, CronExpression cronExp, MethodInfo methodInfo, DateTime? lastTrigger)
{
	public ScheduleAttribute Info { get; private set; } = scheduleInfo;
	public CronExpression CronExpression { get; private set; } = cronExp;
	public MethodInfo MethodInfo { get; private set; } = methodInfo;

	/// <summary>
	/// Indicator if the regex allows for a catchup of missed schedules
	/// </summary>
	public bool CanCatchup =>
		Info.HistoryCatchup is not (CATCHUP.NONE or CATCHUP.NEVER);

	/// <summary>
	/// A schedule is considered volatile, if it does not keep track of a history
	/// In this case, a
	/// </summary>
	public bool IsVolatile => Info.HistoryCatchup == CATCHUP.NONE;
	public DateTime? LastTrigger { get; set; } = lastTrigger;
	public bool HasBeenTriggered => LastTrigger != null;
	public Timer? Timer;

	// Milliseconds until the cronexpression resolves based on cron expression
	public long MsToNext => CronExpression.GetNextOccurrence(DateTimeOffset.Now, TimeZoneInfo.Local)?.ToUnixTimeMilliseconds() -
				DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() ?? 0;

	// A parsed info text for consoles and debug
	public string InfoText =>
			@$"Schedule {Info.Id} -
			Next: {DateTimeHelper.FormatDuration((int)MsToNext / 1000)} -
			Last: {(LastTrigger == null ? "None" : LastTrigger.Value.ToLongTimeString())}";
}


/// <summary>
/// Defines a scheduled method with a given id and a cron expression
/// If you want something to run every day at 16:00 you can use something like "0 16 * * *"
///
/// IMPORTANT: The seconds are OMITTED
///
/// See https://github.com/HangfireIO/Cronos
/// </summary>
/// <param name="id"></param>
/// <param name="cronExp"></param>
/// <param name="historyCatchup"></param>
[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class ScheduleAttribute(SCHEDULE id, string cronExp, CATCHUP historyCatchup = CATCHUP.NEVER) : Attribute
{
	public SCHEDULE Id { get; private set; } = id;
	public string CronExp { get; private set; } = cronExp;
	public CATCHUP HistoryCatchup { get; private set; } = historyCatchup;
}


namespace Pillars.Schedules.Models;

/// <summary>
/// A Cycle describes a method to be run in a cyclic manner, e.g. every 15 minutes.
/// All cycles are bound to a common timer.
/// </summary>
public sealed class Cycle(
	CycleAttribute cycleInfo,
	MethodInfo methodInfo,
	CycleTracker cycleTracker,
	DateTime? lastTrigger)
{
	public CycleAttribute Info { get; private set; } = cycleInfo;
	public MethodInfo MethodInfo { get; private set; } = methodInfo;
	public CycleTracker CycleTracker { get; private set; } = cycleTracker;

	/// <summary>
	/// Flag to indicate if this cycle is currently being triggered.
	/// The cycletimer may elapse while the cycle is already triggered, so this is needed
	/// </summary>
	public bool IsBeingTriggered;

	/// <summary>
	/// The minutes until the next trigger
	/// </summary>
	public uint MinutesToNext =>
		Info.CycleTime - CycleTracker.ElapsedMinutes;

	/// <summary>
	/// Indicator if the regex allows for a catchup of missed schedules
	/// </summary>
	public bool CanCatchup =>
		Info.HistoryCatchup is not (CATCHUP.NONE or CATCHUP.NEVER);

	/// <summary>
	/// A cycle is considered volatile, if it does not keep track of a history.
	/// </summary>
	public bool IsVolatile => Info.HistoryCatchup == CATCHUP.NONE;

	/// <summary>
	/// DateTime the last time this was triggered since server start
	/// </summary>
	public DateTime? LastTrigger { get; private set; } = lastTrigger;

	public bool HasHistory => LastTrigger != null;

	/// <summary>
	/// The trigger count during THIS runtime (resets to 0 after server start. Does not include catchups)
	/// </summary>
	public uint TriggerCount { get; private set; }

	/// <summary>
	/// Sets the last trigger and increments the counter
	/// </summary>
	/// <param name="lastTrigger"></param>
	/// <param name="incrementCount"></param>
	public void SetLastTrigger(DateTime lastTrigger, bool incrementCount = true)
	{
		if (incrementCount) TriggerCount++;
		LastTrigger = lastTrigger;
	}

	// A parsed info text for consoles and debug
	public string InfoText =>
		@$"Cycle {Info.Id} -
			MinutesToNext: {MinutesToNext} -
			Triggers: {TriggerCount} -
			Last: {(LastTrigger == null ? "None" : LastTrigger.Value.ToLongTimeString())}";
}

/// <summary>
/// Defines a cyclic method with a given id and some creation flags.
/// Use this if you want to run something every 2 hours and don't care about the WHEN.
///
/// If the WHEN is important, use a Schedule!
/// </summary>
/// <remarks>
/// Only allows for 2 or more Minutes !
/// </remarks>
/// <param name="id"></param>
/// <param name="cycleTimeInMinutes">The cycle time in minutes</param>
/// <param name="triggerOnCreate">If the cycle is new (no history) it will be triggered on start</param>
/// <param name="historyCatchup">How the history should be handled</param>
/// <param name="resetElapsedOnStart">If set, resets the elapsed minutes on start</param>
[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class CycleAttribute(
	CYCLE id,
	uint cycleTimeInMinutes,
	bool triggerOnCreate = false,
	CATCHUP historyCatchup = CATCHUP.NEVER,
	bool resetElapsedOnStart = true) : Attribute
{
	public CYCLE Id { get; private set; } = id;
	public uint CycleTime { get; private set; } = cycleTimeInMinutes;
	public CATCHUP HistoryCatchup { get; private set; } = historyCatchup;
	public bool TriggerOnCreate { get; private set; } = triggerOnCreate;
	public bool ResetElapsedOnStart { get; private set; } = resetElapsedOnStart;
}

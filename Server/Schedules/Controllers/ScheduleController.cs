namespace Pillars.Schedules.Controllers;

[RegisterSingleton(typeof(ScheduleController))]
public sealed class ScheduleController : IController
{
	private readonly ILogger _logger;
	private readonly ScheduleService _scheduleService;
	private readonly IServiceProvider _serviceProvider;

	#region Ctor
	public ScheduleController(ScheduleService scheduleService, IServiceProvider serviceProvider, ILogger logger, ServerEvents serverEvents)
	{
		_logger = logger.ForThisContext();
		_scheduleService = scheduleService;
		_serviceProvider = serviceProvider;
		serverEvents.OnServerStarted += StartSchedules;
		serverEvents.OnServerStarted += StartCycles;
	}
	#endregion


	#region INITIALIZE
	/// <summary>
	/// Initializes the Schedule Controller by loading all available Schedules.
	/// </summary>
	public async Task InitializeAsync()
	{
		Schedules = await _scheduleService.LoadSchedulesAsync();
		_logger.Information("Loaded a total of {count} schedules", Schedules.Count);
		Cycles = await _scheduleService.LoadCyclesAsync();
		_logger.Information("Loaded a total of {count} cycles", Cycles.Count);
	}

	#endregion

	#region SCHEDULES
	public List<Schedule> Schedules { get; private set; } = [];

	/// <summary>
	/// For a given id returns the appropriate schedule
	/// </summary>
	public Schedule? GetScheduleById(uint id) =>
		GetScheduleById((SCHEDULE) id);

	/// <summary>
	/// For a given SCHEDULE id returns the appropriate schedule
	/// </summary>
	public Schedule? GetScheduleById(SCHEDULE id) =>
		Schedules.FirstOrDefault((schedule) => schedule.Info.Id == id);


	/// <summary>
	///  Triggers the schedule catchup sequence and
	///  on success, starts all timers for all schedules
	/// </summary>
	private async Task StartSchedules()
	{
		await PerformScheduleCatchups();
		foreach(var schedule in Schedules)
		{
			StartScheduleTimer(schedule);
		}
	}

	/// <summary>
	/// Foreach schedule, checks if one or more schedules were missed
	/// while being offline. Based on the catchup setting of the schedule, either
	/// performs all, one or none missed schedules.
	/// </summary>
	private async Task PerformScheduleCatchups()
	{
		var schedules = Schedules
			.FindAll((s) => s.CanCatchup && s.HasBeenTriggered);
		if (schedules.Count == 0)
		{
			_logger.Information("No schedules to perform a catchup for");
			return;
		}

		foreach (var sched in schedules)
		{
			var missedTriggers = ScheduleHelper.OccurrencesSince(sched.CronExpression, (DateTime)sched.LastTrigger!);
			if (missedTriggers <= 0) continue;

			if (sched.Info.HistoryCatchup == CATCHUP.ONCE)
			{
				// Perform one trigger to catchup all missing
				_logger.Information("Performing catchup of Schedule {id} ONCE - Total missed triggers: {count}", sched.Info.Id, missedTriggers);
				if (!await TriggerScheduleCatchup(sched, $"catchup once - missed {missedTriggers}"))
					throw new Exception($"Catchup ONCE of schedule {sched.Info.Id} failed");
				else
					_logger.Information("Catchup of Schedule {schedule} for {count} missed triggers completed!", sched.Info.Id, missedTriggers);
			}

			if (sched.Info.HistoryCatchup == CATCHUP.ALL)
			{
				// Looping all missed triggers
				_logger.Information("Performing catchup of Schedule {schedule} ALL - Total missed triggers: {count}", sched.Info.Id, missedTriggers);
				for(var i = 1; i <= missedTriggers; i++)
				{
					if (!await TriggerScheduleCatchup(sched, $"catchup all - {i}/{missedTriggers}"))
						throw new Exception($"Catchup {i}/{missedTriggers} of schedule {sched.Info.Id} failed");
					else
						_logger.Information("Catchup {i}/{count} of Schedule {schedule} completed!", i, missedTriggers, sched.Info.Id);
				}
			}
		}
	}

	/// <summary>
	/// Triggers a schedule in the catchup context,
	/// e.g. does not start / stop the timer but also creates a history.
	/// Returns the result of the schedule
	/// </summary>
	private async Task<bool> TriggerScheduleCatchup(Schedule schedule, string reason)
	{
		try
		{
			var result = await TriggerSchedule(schedule);
			schedule.LastTrigger = schedule.IsVolatile ? DateTime.UtcNow : (await _scheduleService.CreateScheduleHistory(schedule, reason, result)).CreatedAt;
			return result;
		}
		catch (Exception ex)
		{
			_logger.Error(ex);
			return false;
		}

	}

	/// <summary>
	/// For a given schedule, starts the timer and registers
	/// the trigger upon elapse
	/// </summary>
	private void StartScheduleTimer(Schedule schedule)
	{
		try
		{
			StopScheduleTimer(schedule);
			// Ms for the timer to trigger
			var msToNext = schedule.MsToNext;
			if (msToNext <= 0)
			{
				_logger.Warning("Schedule {id} calculated time to next is <=0 , triggering...", schedule.Info.Id);
				TriggerSchedule(schedule, "TimeToNext <= 0");
			}
			else
			{
				schedule.Timer = new();
				schedule.Timer.Elapsed += async (_, _) => TriggerSchedule(schedule , "automatic");
				schedule.Timer.Interval = msToNext;
				_logger.Information("Schedule {id} scheduled for trigger in {dhms}", schedule.Info.Id, DateTimeHelper.FormatDuration((int)msToNext / 1000));
				schedule.Timer.Start();
			}
		}
		catch (Exception ex)
		{
			_logger.Error(ex);
		}
	}

	/// <summary>
	/// For a given schedule, stops the timer and
	/// disposes it.
	/// </summary>
	/// <param name="schedule"></param>
	private void StopScheduleTimer(Schedule schedule)
	{
		try
		{
			if (schedule.Timer == null) return;
			schedule.Timer.Stop();
			schedule.Timer.Dispose();
			schedule.Timer = null;
		}
		catch (Exception ex)
		{
			_logger.Error(ex);
		}
	}

	/// <summary>
	/// Triggers a specific schedule, creates a history entry
	/// </summary>
	public async Task<bool> TriggerSchedule(Schedule schedule, string reason)
	{
		try
		{
			_logger.Information("Schedule {id} triggered - Reason: {reason}", schedule.Info.Id, reason);
			StopScheduleTimer(schedule);
			var result = await TriggerSchedule(schedule);
			schedule.LastTrigger = schedule.IsVolatile ? DateTime.UtcNow : (await _scheduleService.CreateScheduleHistory(schedule, reason, result)).CreatedAt;
			return result;
		}
		catch (Exception ex)
		{
			_logger.Error(ex);
			return false;
		}
		finally
		{
			StartScheduleTimer(schedule);
		}
	}

	/// <summary>
	/// Internally triggers a schedule. Returns the result of the schedule.
	/// Starting and stopping timer is not handled explicitly.
	/// </summary>
	private async Task<bool> TriggerSchedule(Schedule schedule)
	{
		try
		{
			if (schedule.MethodInfo.DeclaringType is null)
				return false;

			var obj = _serviceProvider.GetServices(schedule.MethodInfo.DeclaringType).FirstOrDefault() ??
						Activator.CreateInstance(schedule.MethodInfo.DeclaringType);
			object result = schedule.MethodInfo.Invoke(obj, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, null, null)!;
			return result is Task<bool> task ? await task : result is bool v && v;
		}
		catch (Exception ex)
		{
			_logger.Error(ex);
			return false;
		}
	}

	#endregion

	#region CYCLES
	public List<Cycle> Cycles { get; private set; } = [];

	/// <summary>
	///  Triggers the cycle catchup sequence and
	///  on success, starts the generic Cycle Timer
	/// </summary>
	private async Task StartCycles()
	{
		await PerformCycleCatchups();
		foreach(var cycle in Cycles)
			_logger.Information("Cycle {id} scheduled for trigger in {remain}", cycle.Info.Id, DateTimeHelper.FormatDuration((int)cycle.MinutesToNext * 60));

		StartCycleTimer();
	}

	/// <summary>
	/// Foreach cycle, checks if one or more cycles were missed
	/// while being offline. Based on the catchup setting of the cycle, either
	/// performs all, one or none missed cycles.
	/// </summary>
	private async Task PerformCycleCatchups()
	{
		// Catchup Sequence
		foreach (var cycle in Cycles)
		{
			if (!cycle.HasHistory && cycle.Info.TriggerOnCreate)
			{
				_logger.Information("Cycle {id} is new and registered ot trigger on create. Triggering...");
				if (!await TriggerCycleCatchup(cycle, "initial"))
					throw new Exception($"Initial trigger of cycle {cycle.Info.Id} failed");
			}
			else if (cycle.CanCatchup)
			{
				var missedTriggers = ScheduleHelper.GetMissedCycleTriggers(cycle);
				if (missedTriggers <= 0) continue;

				if (cycle.Info.HistoryCatchup == CATCHUP.ONCE)
				{
					// Perform one trigger to catchup all missing
					_logger.Information("Performing catchup of Cycle {id} ONCE - Total missed triggers: {count}", cycle.Info.Id, missedTriggers);
					if (!await TriggerCycleCatchup(cycle, $"catchup once - missed {missedTriggers}"))
						throw new Exception($"Catchup ONCE of Cycle {cycle.Info.Id} failed");
					else
						_logger.Information("Catchup of Cycle {id} for {count} missed triggers completed!", cycle.Info.Id, missedTriggers);
				}

				if (cycle.Info.HistoryCatchup == CATCHUP.ALL)
				{
					// Looping all missed triggers
					_logger.Information("Performing catchup of Cycle {id} ALL - Total missed triggers: {count}", cycle.Info.Id, missedTriggers);
					for(var i = 1; i <= missedTriggers; i++)
					{
						if (!await TriggerCycleCatchup(cycle, $"catchup all - {i}/{missedTriggers}"))
							throw new Exception($"Catchup {i}/{missedTriggers} of Cycle {cycle.Info.Id} failed");
						else
							_logger.Information("Catchup {i}/{count} of Cycle {id} completed!", i, missedTriggers, cycle.Info.Id);
					}
				}
			}
		}

		// Reset the ElapsedTime if applicable by cycle
		var resetElapsedCycles = Cycles.FindAll((cyc) => cyc.Info.ResetElapsedOnStart);
		if (resetElapsedCycles == null) return;

		foreach (var cycle in resetElapsedCycles)
		{
			_logger.Information("Resetting ElapsedMinutes of Cycle {id}", cycle.Info.Id);
			cycle.CycleTracker.ElapsedMinutes = 0;
		}
		await _scheduleService.UpdateCycleTrackers([.. resetElapsedCycles.Select((cyc) => cyc.CycleTracker)]);
	}

	/// <summary>
	/// Triggers a cycle in the catchup context,
	/// Returns the result of the cycle
	/// </summary>
	private async Task<bool> TriggerCycleCatchup(Cycle cycle, string reason)
	{
		try
		{
			var result = await InternalCycleTrigger(cycle);
			cycle.SetLastTrigger(
				cycle.IsVolatile ? DateTime.UtcNow : (await _scheduleService.CreateCycleHistory(cycle, reason, result)).CreatedAt,
				false
			);
			return result;
		}
		catch (Exception ex)
		{
			_logger.Error(ex);
			return false;
		}

	}

	/// <summary>
	/// Triggers a specific cycle, creates a history entry.
	/// DOES NOT reset the elapsed minutes of the CycleTracker to 0.
	/// The cycle timer does this before a trigger
	/// </summary>
	public async Task<bool> TriggerCycle(Cycle cycle, string reason, bool automatic)
	{
		try
		{
			if (cycle.IsBeingTriggered) return false;

			cycle.IsBeingTriggered = true;
			_logger.Information("Cycle {id} triggered - Reason: {reason}", cycle.Info.Id, reason);
			var result = await InternalCycleTrigger(cycle);
			cycle.SetLastTrigger(cycle.IsVolatile && automatic ? DateTime.UtcNow : (await _scheduleService.CreateCycleHistory(cycle, reason, result)).CreatedAt);
			// Resetting elapsed minutes if cycle
			if (!automatic)
				cycle.CycleTracker.ElapsedMinutes = 0;
			_logger.Information("Cycle {id} - Next Trigger in {time}", cycle.Info.Id, DateTimeHelper.FormatDuration((int)cycle.MinutesToNext * 60));
			return result;
		}
		catch (Exception ex)
		{
			_logger.Error(ex);
			return false;
		}
		finally
		{
			cycle.IsBeingTriggered = false;
		}
	}

	/// <summary>
	/// Internally triggers a cycle. Returns the result of the cycle.
	/// </summary>
	private async Task<bool> InternalCycleTrigger(Cycle cycle)
	{
		try
		{
			if (cycle.MethodInfo.DeclaringType is null)
				return false;

			var obj = _serviceProvider.GetServices(cycle.MethodInfo.DeclaringType).FirstOrDefault() ??
						Activator.CreateInstance(cycle.MethodInfo.DeclaringType);
			object result = cycle.MethodInfo.Invoke(obj, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance, Type.DefaultBinder, null, null)!;
			return result is Task<bool> task ? await task : result is bool v && v;
		}
		catch (Exception ex)
		{
			_logger.Error(ex);
			return false;
		}
	}

	/// <summary>
	/// For a given cycle id returns the appropriate cycle
	/// </summary>
	public Cycle? GetCycleById(uint id) =>
		GetCycleById((CYCLE) id);

	/// <summary>
	/// For a given cycle id returns the appropriate cycle
	/// </summary>
	public Cycle? GetCycleById(CYCLE id) =>
		Cycles.FirstOrDefault((cyc) => cyc.Info.Id == id);
	#endregion

	#region CYCLE TIMER
	private PeriodicTimer? _cycleTimer;

	/// <summary>
	/// Starts the internal cycle timer
	/// </summary>
	private async Task StartCycleTimer()
	{
		if (_cycleTimer != null) return;
		_cycleTimer = new(TimeSpan.FromMinutes(1));
		CycleTimerElapsed();
	}

	/// <summary>
	/// Triggered whe nthe cycle timer of 1 minute is elapsed.
	/// Iterates over all cycles and increments the tracker by 1 minute.
	/// If tracker reaches cycle time, the cycle is triggered.
	/// If the cycle is already being triggered, cycle is delayed by one minute.
	/// </summary>
	private async Task CycleTimerElapsed()
	{
		try {
			if (_cycleTimer == null) return;
			while (await _cycleTimer.WaitForNextTickAsync())
			{
				foreach(var cycle in Cycles)
				{
					cycle.CycleTracker.ElapsedMinutes += 1;
					if (cycle.CycleTracker.ElapsedMinutes < cycle.Info.CycleTime) continue;
					else if (cycle.IsBeingTriggered)
					{
						_logger.Warning("Cycle {id} elapsed but is already being triggered. Delaying...");
						cycle.CycleTracker.ElapsedMinutes -= 1;
					}
					else
					{
						cycle.CycleTracker.ElapsedMinutes = 0;
						await TriggerCycle(cycle, "automatic", true);
					}
				}
				await _scheduleService.UpdateCycleTrackers([.. Cycles.Select((c) => c.CycleTracker)]);
			}

		}
		catch (Exception ex)
		{
			_logger.Error(ex);
		}
	}
	#endregion
}

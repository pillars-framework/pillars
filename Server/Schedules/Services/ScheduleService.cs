namespace Pillars.Schedules.Services;

[RegisterSingleton]
public sealed class ScheduleService(ILogger logger)
{
	private readonly ILogger _logger = logger.ForThisContext();

	#region SCHEDULE
	/// <summary>
	/// Performs a lookup over the assembly to check for all [Schedule] attributes.
	/// Also loads the latest history entry for all schedule
	/// If one is found, checks for correctness and creates the schedule.
	/// </summary>
	public async Task<List<Schedule>> LoadSchedulesAsync()
	{
		ConcurrentDictionary<SCHEDULE, Schedule> schedules = [];
		var histories = await LoadLatestHistoryForEachSchedule();

		var methods = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(x => x.GetTypes())
				.Where(x => x.IsClass)
				.SelectMany(x => x.GetMethods())
				.Where(x => x.GetCustomAttributes(typeof(ScheduleAttribute), false).FirstOrDefault() != null)
				.ToList();

		foreach(var mi in methods)
		{
			var attr = (ScheduleAttribute?)Attribute.GetCustomAttribute(mi, typeof(ScheduleAttribute));
			if (attr == null) continue;
			else if (schedules.ContainsKey(attr.Id))
				throw new ArgumentException($"Schedule {attr.Id} is already a registered schedule!");
			else if (string.IsNullOrEmpty(attr.CronExp))
				throw new ArgumentException($"Schedule {attr.Id} does not provide a cron expression!");
			else if (!CronExpression.TryParse(attr.CronExp, out CronExpression cron))
				throw new ArgumentException($"Schedule {attr.Id} does not provide a valid cron expression!");
			else if (!ScheduleHelper.IsValidCronExpression(cron))
				throw new ArgumentException($"Schedule {attr.Id} does not provide a valid cron expression for a next iterator!");
			else if (!(mi.ReturnType == typeof(bool) || mi.ReturnType == typeof(Task<bool>)))
				throw new Exception($"Schedule {attr.Id} does not return a Task<bool> or bool as a result!");
			else
			{
				var lastHistory = histories.FirstOrDefault((h) => h.ScheduleId == attr.Id);
				Schedule schedule = new(attr, cron, mi, lastHistory?.CreatedAt);
				if (!schedules.TryAdd(attr.Id, schedule))
					throw new ArgumentException($"Schedule {attr.Id} is already a registered schedule!");
			}
		}

		return [.. schedules.Values];
	}

	/// <summary>
	/// Loads the latest history entries for each existing schedule
	/// in the database. Returns a list containing all latest schedule histories
	/// </summary>
	private async Task<List<ScheduleHistory>> LoadLatestHistoryForEachSchedule() => await DB.Collection<ScheduleHistory>()
		.Aggregate()
		.Group(
			h => h.ScheduleId, // Group by ScheduleId
			g => new
			{
				Id = g.Key,
				LatestHistory = g.OrderByDescending(h => h.CreatedAt).First() // Get the latest entry
			}
		)
		.Project(g => g.LatestHistory) // Extract only the latest entry from each group
		.ToListAsync();

	/// <summary>
	/// Creates a History entry for the given schedule
	/// </summary>
	public async Task<ScheduleHistory> CreateScheduleHistory(Schedule schedule, string reason, bool successful)
	{
		var history = new ScheduleHistory
		{
			ScheduleId = schedule.Info.Id,
			Reason = reason,
			Success = successful,
			CreatedAt = DateTime.UtcNow
		};

		try
		{
			await history.SaveAsync();
			return history;
		}
		catch (Exception ex)
		{
			_logger.Error(ex);
			return history;
		}
	}
	#endregion

	#region CYCLE
	/// <summary>
	/// Performs a lookup over the assembly to check for all [Cycle] attributes.
	/// Also loads the latest history entry for all cycles
	/// If one is found, checks for correctness and creates the cycle.
	/// </summary>
	public async Task<List<Cycle>> LoadCyclesAsync()
	{
		ConcurrentDictionary<CYCLE, Cycle> cycles = [];
		var histories = await LoadLatestHistoryForEachCycle();
		var trackers = await LoadCycleTrackers();

		var methods = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(x => x.GetTypes())
				.Where(x => x.IsClass)
				.SelectMany(x => x.GetMethods())
				.Where(x => x.GetCustomAttributes(typeof(CycleAttribute), false).FirstOrDefault() != null)
				.ToList();

		foreach(var mi in methods)
		{
			var attr = (CycleAttribute?)Attribute.GetCustomAttribute(mi, typeof(CycleAttribute));
			if (attr == null) continue;
			else if (cycles.ContainsKey(attr.Id))
				throw new ArgumentException($"Cycle {attr.Id} is already a registered schedule!");
			else if (attr.CycleTime < 1)
				throw new ArgumentException($"Cycle {attr.Id} has unsupported cycletime (< 1) !");
			else if (!(mi.ReturnType == typeof(bool) || mi.ReturnType == typeof(Task<bool>)))
				throw new Exception($"Cycle {attr.Id} does not return a Task<bool> or bool as a result!");
			else
			{
				var tracker = trackers.FirstOrDefault((ct) => ct.CycleId == attr.Id);
				tracker ??= await CreateCycleTracker(attr.Id);
				var lastHistory = histories.FirstOrDefault((h) => h.CycleId == attr.Id);
				Cycle cycle = new(attr, mi, tracker, lastHistory?.CreatedAt);
				if (!cycles.TryAdd(attr.Id, cycle))
					throw new ArgumentException($"Cycle {attr.Id} is already a registered cycle!");
			}
		}

		return [.. cycles.Values];
	}

	/// <summary>
	/// Loads the latest history entries for each existing cycle
	/// in the database. Returns a list containing all latest cycle histories
	/// </summary>
	private async Task<List<CycleHistory>> LoadLatestHistoryForEachCycle() => await DB.Collection<CycleHistory>()
		.Aggregate()
		.Group(
			h => h.CycleId, // Group by CycleId
			g => new
			{
				Id = g.Key,
				LatestHistory = g.OrderByDescending(h => h.CreatedAt).First() // Get the latest entry
			}
		)
		.Project(g => g.LatestHistory) // Extract only the latest entry from each group
		.ToListAsync();

	/// <summary>
	/// Creates a History entry for the given cycle
	/// </summary>
	public async Task<CycleHistory> CreateCycleHistory(Cycle cycle, string reason, bool successful)
	{
		CycleHistory history = new(){
			CycleId = cycle.Info.Id,
			Reason = reason,
			Success = successful,
			CreatedAt = DateTime.UtcNow
		};
		try
		{
			history.SaveAsync();
			return history;
		}
		catch (Exception ex)
		{
			_logger.Error(ex);
			return history;
		}
	}

	/// <summary>
	/// Loads all existing cycle trackers
	/// </summary>
	private async Task<List<CycleTracker>> LoadCycleTrackers() => await DB.Find<CycleTracker>().ExecuteAsync();

	/// <summary>
	/// For a given cycle ID, creates a cycle tracker.
	/// Checks if an entry exists for the given tracker
	/// and returns the tracker if in doubt to avoid unique constraint fails
	/// </summary>
	private async Task<CycleTracker> CreateCycleTracker(CYCLE cycle)
	{
		// Try to find an existing tracker
		var tracker = await DB.Find<CycleTracker>()
			.Match(ct => ct.CycleId == cycle)
			.ExecuteFirstAsync();

		if (tracker != null)
			return tracker;

		tracker = new()
		{
			CycleId = cycle,
			ElapsedMinutes = 0
		};

		await tracker.SaveAsync();
		return tracker;
	}

	/// <summary>
	/// Updates the cycle trackers in the database.
	/// Mainly used to update the elapsed minutes
	/// </summary>
	public async Task UpdateCycleTrackers(List<CycleTracker> cycleTrackers) => await DB.SaveAsync(cycleTrackers);
	#endregion
}

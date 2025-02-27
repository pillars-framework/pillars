namespace Pillars.Schedules.Models;

[Collection("schedule_history")]
public sealed class ScheduleHistory : Entity, ICreatedOn
{
	/// <summary>
	/// The schedule for this history entry.
	/// </summary>
	public SCHEDULE ScheduleId { get; set; }

	/// <summary>
	/// Reason why the schedule was triggered.
	/// </summary>
	public string Reason { get; set; } = "automatic";

	/// <summary>
	/// If the schedule was successfully performed.
	/// </summary>
	public bool Success { get; set; } = true;

	public DateTime CreatedOn { get; set; }
}

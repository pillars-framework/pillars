namespace Pillars.Schedules.Models;

/// <summary>
/// This table tracks cycle and their runtime.
/// Only one entry per cycle is allowed
/// </summary>
[Collection("cycle_tracker")]
public sealed class CycleTracker : Entity
{
	/// <summary>
	/// The type of the cycle.
	/// </summary>
	[BsonRequired]
	public CYCLE CycleId { get; set; }

	/// <summary>
	/// Minutes that have elapsed in the CURRENT cycle.
	/// </summary>
	[BsonRequired]
	public uint ElapsedMinutes { get; set; }
}

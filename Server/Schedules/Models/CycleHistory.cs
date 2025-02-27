namespace Pillars.Schedules.Models;

[Collection("cycle_history")]
public sealed class CycleHistory : Entity, ICreatedOn
{
	/// <summary>
	/// The cycle for this history entry.
	/// </summary>
	public CYCLE CycleId { get; set; }

	/// <summary>
	/// Reason why the cycle was triggered.
	/// </summary>
	public string Reason { get; set; } = "automatic";

	/// <summary>
	/// If the cycle was successfully performed.
	/// </summary>
	public bool Success { get; set; } = true;

	public DateTime CreatedOn { get; set; }
}

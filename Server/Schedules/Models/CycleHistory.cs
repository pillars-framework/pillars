namespace Pillars.Schedules.Models;

[Collection("cycle_history")]
public sealed class CycleHistory : Entity
{
	/// <summary>
	/// The cycle for this history entry.
	/// </summary>
	public CYCLE CycleId { get; set; }

	/// <summary>
	/// Creation Date.
	/// </summary>
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

	/// <summary>
	/// Reason why the cycle was triggered.
	/// </summary>
	public string Reason { get; set; } = "automatic";

	/// <summary>
	/// If the cycle was successfully performed.
	/// </summary>
	public bool Success { get; set; } = true;
}

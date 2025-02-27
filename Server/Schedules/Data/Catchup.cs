namespace Pillars.Schedules.Data;

/// <summary>
/// The enum describes the behavior what should happen if a schedule or cycle
/// was missed, while the server was offline.
/// </summary>
public enum CATCHUP
{
	/// <summary>
	/// No History will be added for this schedule
	/// </summary>
	NONE = 0,

	/// <summary>
	/// Does nothing when server starts, but keeps a history in the database
	/// </summary>
	NEVER = 1,

	/// <summary>
	/// If any number of cycles or schedules were missed, the method will be run ONCE as a catchup
	/// </summary>
	ONCE = 2,

	/// <summary>
	/// Performs the schedule / cycle for exactly the amounts of missed cycles or schedules
	/// </summary>
	ALL = 3
}

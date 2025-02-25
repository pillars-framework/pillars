namespace Pillars.Schedules.Data;

/// <summary>
/// The enum describes the behavior what should happen if a schedule or cycle
/// was missed, while the server was offline.
/// </summary>
public enum CATCHUP
{
	NONE = 0, // No History will be added for this schedule
	NEVER = 1, // Does nothing when server starts, but keeps a history in the database
	ONCE = 2, // If any number of cycles or schedules was missed, the method will be run ONCE as a catchup
	ALL = 3 // Performs the schedule / cycle for exactly the amounts of missed cycles or schedules
}

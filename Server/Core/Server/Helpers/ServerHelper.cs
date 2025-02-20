namespace Pillars.Core.Server.Helpers;

/// <summary>
/// Helper class to determine string representations etc.
/// </summary>
public static class ServerHelper
{
	/// <summary>
	/// For a given server state, returns the string representation
	/// </summary>
	public static string GetServerStateAsString(SERVERSTATE state) =>
		state switch
			{
				SERVERSTATE.RUNNING => "RUNNING",
				SERVERSTATE.STARTING => "STARTING",
				SERVERSTATE.STOPPING => "STOPPING",
				_ => "UNKNOWN",
			};
}

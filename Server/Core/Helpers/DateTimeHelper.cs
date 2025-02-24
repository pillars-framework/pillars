namespace Pillars.Core.Helpers;

public static class DateTimeHelper
{
	/// <summary>
	/// Formats a given amuount of seconds into a displayable string
	/// </summary>
	public static string FormatDuration(int seconds)
	{
		var d = seconds / 86400;
		seconds %= 86400;
		var h = seconds / 3600;
		seconds %= 3600;
		var m = seconds / 60;
		seconds %= 60;
		return (d > 0 ? d + " Day(s) " : "") + (h > 0 ? h + " H. " : "") + (m > 0 ? m + " min. " : "") +
		       (seconds > 0 ? seconds + " sec." : "");
	}

	#region UNIX / EPOCH CONVERSION

	public static readonly DateTime UnixEpoch = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

	/// <summary>
	/// Takes a DateTime and returns the milliseconds since 01.01.1970
	/// </summary>
	public static long ToUnixTimeMilliseconds(DateTime dateTime) =>
		(long)(dateTime.ToUniversalTime() - UnixEpoch).TotalMilliseconds;

	/// <summary>
	/// Takes the milliseconds from 01.01.1970 and returns a valid DateTime
	/// </summary>
	public static DateTime FromUnixTimeMilliseconds(long unixTimeMilliseconds) =>
		UnixEpoch.AddMilliseconds(unixTimeMilliseconds);

	#endregion
}

namespace Pillars.Notifications.Actors;

[RegisterSingleton]
public sealed class NotificationActor : PiActor<BpNotification>
{
	/// <summary>
	/// Notifies a set of player
	/// </summary>
	/// <param name="targets">The players to notify</param>
	/// <param name="icon"><see cref="NOTIFICATIONICON"/></param>
	/// <param name="title">Title of the notification (first line)</param>
	/// <param name="text">Text of the notification (second line)</param>
	/// <param name="duration">Duration in seconds to display the notification</param>
	/// <remarks>The notification duration is clamped at minimum 1 second on client</remarks>
	public async Task Notify(IEnumerable<PiPlayer> targets, NOTIFICATIONICON icon, string title, string text,
		float duration = 5.0f) =>
		Notify(targets, (int)icon, title, text, duration);

	/// <summary>
	/// Notifies a set of player
	/// </summary>
	/// <param name="targets">The players to notify</param>
	/// <param name="iconIdx">The icon index of the clientside texture array</param>
	/// <param name="title">Title of the notification (first line)</param>
	/// <param name="text">Text of the notification (second line)</param>
	/// <param name="duration">Duration in seconds to display the notification</param>
	/// <remarks>The notification duration is clamped at minimum 1 second on client</remarks>
	public async Task Notify(IEnumerable<PiPlayer> targets, int iconIdx, string title, string text,
		float duration = 5.0f)
	{
		foreach (var target in targets)
			Notify(target, iconIdx, title, text, duration);
	}

	/// <summary>
	/// Sends a notification to a given target.
	/// </summary>
	/// <param name="target">The player to send the notification to</param>
	/// <param name="icon"><see cref="NOTIFICATIONICON"/></param>
	/// <param name="title">Title of the notification (first line)</param>
	/// <param name="text">Text of the notification (second line)</param>
	/// <param name="duration">Duration in seconds to display the notification</param>
	/// <remarks>The notification duration is clamped at minimum 1 second on client</remarks>
	public async Task Notify(PiPlayer target, NOTIFICATIONICON icon, string title, string text,
		float duration = 5.0f) =>
		Notify(target, (int)icon, title, text, duration);

	/// <summary>
	/// Sends a notification to a given target.
	/// </summary>
	/// <param name="target">The player to send the notification to</param>
	/// <param name="iconIdx">The icon index of the clientside texture array</param>
	/// <param name="title">Title of the notification (first line)</param>
	/// <param name="text">Text of the notification (second line)</param>
	/// <param name="duration">Duration in seconds to display the notification</param>
	/// <remarks>The notification duration is clamped at minimum 1 second on client</remarks>
	public async Task Notify(PiPlayer target, int iconIdx, string title, string text,
		float duration = 5.0f) =>
		_worldActor.TriggerNotification(target.Native, iconIdx, title, text, duration);
}

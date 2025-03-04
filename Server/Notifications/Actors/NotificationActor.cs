namespace Pillars.Notifications.Actors;

[RegisterSingleton]
public sealed class NotificationActor : PiActor<BpNotification>
{
	public async Task Test(PiPlayer player, string title, string text) =>
		_worldActor.TriggerNotification(player.Native, title, text, 5f);
}

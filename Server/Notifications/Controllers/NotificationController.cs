namespace Pillars.Notifications.Controllers;

using Actors;

[RegisterSingleton]
public sealed class NotificationController(ILogger l, NotificationActor actor)
{
	private readonly ILogger _logger = l.ForThisContext();

	[SlashCommand("notify")]
	private async Task TestNotification(PiPlayer player, string[] args)
	{
		_logger.Information("Testing notification with args: {a}", JsonSerializer.Serialize(args));
		if (args.Length <= 2) return;
		string title = args[0];
		string message = string.Join(" ", args[1..]);
		actor.Test(player, title, message);
	}
}

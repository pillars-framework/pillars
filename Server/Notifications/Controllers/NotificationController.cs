namespace Pillars.Notifications.Controllers;

[RegisterSingleton]
public sealed class NotificationController(ILogger l, NotificationActor actor)
{
	private readonly ILogger _logger = l.ForThisContext();

	[SlashCommand("notify")]
	private async Task TestNotification(PiPlayer player, string[] args)
	{
		_logger.Information("Testing notification with args: {a}", JsonSerializer.Serialize(args));
		if (args.Length <= 3)
			actor.Notify(player, NOTIFICATIONICON.UI_T_GENERICERROR, "Invalid number of arguments",
				"Expected at least 3 arguments");
		else if (!int.TryParse(args[0], out int iconIdx))
			actor.Notify(player, NOTIFICATIONICON.UI_T_GENERICERROR, "Invalid icon",
				$"\"{args[0]}\" is not a valid icon");
		else
		{
			string title = args[1];
			string message = string.Join(" ", args[2..]);
			actor.Notify(player, iconIdx, title, message);
		}
	}
}

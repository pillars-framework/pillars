namespace Pillars.Test.Controllers;

[RegisterSingleton]
public sealed class TestController
{
	private readonly ILogger _logger;
	private readonly AccountService _accountService;
	private readonly ChatActor _chatActor;
	private readonly NotificationActor _notificationActor;

	public TestController(ILogger l, PlayerConnectionEvents pce, AccountService accountService, ChatActor ca,
		NotificationActor na)
	{
		_chatActor = ca;
		_notificationActor = na;
		_logger = l.ForThisContext();
		_accountService = accountService;
		pce.OnPlayerConnected += OnPlayerConnect;
		pce.OnPlayerDisconnected += OnPlayerDisconnect;
	}

	private async Task OnPlayerConnect(PiPlayer player)
	{
		_logger.Debug("Player {pid} connected - Connection Id: {cid}", player.Id,
			player.ConnectionId);
		await Task.Delay(5_000);
		_logger.Debug("Sending message");
		_chatActor.SendMessage(player, "Hello from test");
	}

	private async Task OnPlayerDisconnect(PiPlayer player)
	{
		_logger.Debug("Player {pid} left", player.Id);
	}

	[SlashCommand("notify")]
	private async Task TestNotification(PiPlayer player, string[] args)
	{
		_logger.Information("Testing notification with args: {a}", JsonSerializer.Serialize(args));
		if (args.Length <= 3)
			_notificationActor.Notify(player, NOTIFICATIONICON.UI_T_GENERICERROR,
				"Invalid number of arguments",
				"Expected at least 3 arguments");
		else if (!int.TryParse(args[0], out int iconIdx))
			_notificationActor.Notify(player, NOTIFICATIONICON.UI_T_GENERICERROR, "Invalid icon",
				$"\"{args[0]}\" is not a valid icon");
		else
		{
			string title = args[1];
			string message = string.Join(" ", args[2..]);
			_notificationActor.Notify(player, iconIdx, title, message);
		}
	}

	/*
	[Cycle(CYCLE.TEST, 1, false, CATCHUP.ALL)]
	public async Task<bool> TestCycle()
	{
		_logger.Information("Testing cycle triggered!");
		return true;
	}

	[Schedule(SCHEDULE.TEST, "* * * * *", CATCHUP.ALL)]
	public async Task<bool> TestSchedule()
	{
		_logger.Information("Testing schedule triggered!");
		return true;
	}
	*/
}

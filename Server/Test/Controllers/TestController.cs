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

	[SlashCommand("chat")]
	private async Task TestChat(PiPlayer player, string[] args)
	{
		var builder = new ChatMessage.Builder();
		builder.AddIcon(CHATICON.GRYFFINDOR);
		builder.AddText("Default ", CHATTEXTSTYLE.DEFAULT);
		builder.AddText("Gryffindor ", CHATTEXTSTYLE.GRYFFINDOR);
		builder.AddText("Hufflepuff ", CHATTEXTSTYLE.HUFFLEPUFF);
		builder.AddText("Ravenclaw ", CHATTEXTSTYLE.RAVENCLAW);
		builder.AddText("Slytherin ", CHATTEXTSTYLE.SLYTHERIN);
		builder.AddText("Admin ", CHATTEXTSTYLE.ADMIN);
		builder.AddText("Dev ", CHATTEXTSTYLE.DEV);
		builder.AddText("Server ", CHATTEXTSTYLE.SERVER);
		builder.AddText("Red ", CHATTEXTSTYLE.RED);
		builder.AddText("Blue ", CHATTEXTSTYLE.BLUE);
		builder.AddText("Green ", CHATTEXTSTYLE.GREEN);
		builder.AddText("Yellow ", CHATTEXTSTYLE.YELLOW);
		builder.AddText("Magenta ", CHATTEXTSTYLE.MAGENTA);
		builder.AddText("Cyan ", CHATTEXTSTYLE.CYAN);
		builder.AddSender(player.Username, CHATTEXTSTYLE.SERVER);
		_chatActor.SendMessage(player, builder.Build().Message);
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

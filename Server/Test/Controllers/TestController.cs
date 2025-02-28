namespace Pillars.Test.Controllers;

[RegisterSingleton]
public sealed class TestController
{
	private readonly ILogger _logger;
	private readonly AccountService _accountService;
	private readonly ChatActor _chatActor;

	public TestController(ILogger l, PlayerConnectionEvents pce, AccountService accountService, ChatActor ca)
	{
		_chatActor = ca;
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

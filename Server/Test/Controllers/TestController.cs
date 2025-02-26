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
		_chatActor.SendMessageToPlayer(player, "Hello from test");
	}

	private async Task OnPlayerDisconnect(PiPlayer player)
	{
		_logger.Debug("Player {pid} left", player.Id);
	}
}

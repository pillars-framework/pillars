namespace Pillars.Test.Controllers;

[RegisterSingleton]
public sealed class TestController
{
	private readonly ILogger _logger;
	private readonly AccountService _accountService;

	public TestController(ILogger l, PlayerConnectionEvents pce, AccountService accountService)
	{
		_logger = l.ForThisContext();
		_accountService = accountService;
		pce.OnPlayerConnected += OnPlayerConnect;
		pce.OnPlayerDisconnected += OnPlayerDisconnect;
	}

	private async Task OnPlayerConnect(PiPlayer player)
	{
		_logger.Debug("Player {pid} connected - Account:", player.Id);
		_logger.Debug("{acc}", JsonSerializer.Serialize(player.Account));
	}

	private async Task OnPlayerDisconnect(PiPlayer player)
	{
		_logger.Debug("Player {pid} left", player.Id);
	}
}

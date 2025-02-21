namespace Pillars.Test.Controllers;

using Core.Accounts;

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
		//var account = await _accountService.CreateAccount(player.DiscordId);

		_logger.Information("Player {pid} joined, perform 30 second wait", player.Id);
		await Task.Delay(30_000);
		_logger.Information("Wait completed for {pid}", player.Id);
	}

	private async Task OnPlayerDisconnect(PiPlayer player) =>
		_logger.Information("Player {pid} left", player.Id);
}

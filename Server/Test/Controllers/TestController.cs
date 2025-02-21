namespace Pillars.Test.Controllers;

[RegisterSingleton]
public sealed class TestController
{
	private readonly ILogger _logger;

	public TestController(ILogger l, PlayerConnectionEvents pce)
	{
		_logger = l.ForThisContext();
		pce.OnPlayerConnected += OnPlayerConnect;
		pce.OnPlayerDisconnected += OnPlayerDisconnect;
	}

	private async Task OnPlayerConnect(PiPlayer player)
	{
		_logger.Information("Player {pid} joined, perform 30 second wait", player.Id);
		await Task.Delay(30_000);
		_logger.Information("Wait completed for {pid}", player.Id);
	}

	private async Task OnPlayerDisconnect(PiPlayer player) =>
		_logger.Information("Player {pid} left", player.Id);
}

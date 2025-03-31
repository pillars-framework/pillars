namespace Pillars.Dialog.Controllers;

[RegisterSingleton]
public sealed class DialogController
{
	private readonly ILogger _logger;
	private readonly DialogActor _dialogActor;

	public DialogController(ILogger l, DialogActor da, PlayerConnectionEvents pce)
	{
		_logger = l.ForThisContext();
		_dialogActor = da;
		pce.OnPlayerDisconnected += OnPlayerDisconnected;
	}

	/// <summary>
	/// If a player disconnects, sets the result of all pending dialogs
	/// to timeout/undefined.
	/// </summary>
	/// <param name="player">The player that disconnected</param>
	private async Task OnPlayerDisconnected(PiPlayer player)
	{
		try
		{
			var pendings = _dialogActor.PendingDialogs.Values.Where(x => x.PlayerDiscordId == player.Id);
			foreach (var pendingDialog in pendings)
				pendingDialog.Tcs.SetResult(DIALOGRESULT.UNDEFINED_TIMEOUT);
		}
		catch (Exception ex)
		{
			_logger.Error(ex);
		}
	}
}

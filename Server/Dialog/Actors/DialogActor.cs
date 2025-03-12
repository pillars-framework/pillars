namespace Pillars.Dialog.Actors;

[RegisterSingleton]
public sealed class DialogActor : PiActor<BpPillarsDialog>
{
	private readonly ConcurrentDictionary<string, TaskCompletionSource<DIALOGRESULT>> _pendingDialogs = new();

	public DialogActor()
	{
		_worldActor.ParentActor = this;
	}

	public void DialogResultReceived(string dialogId, DIALOGRESULT dialogResult)
	{
		if (_pendingDialogs.TryRemove(dialogId, out var request))
			request.SetResult(dialogResult);
	}

	public async Task<DIALOGRESULT> ShowDialogAsync(PiPlayer player, string title, string message)
	{
		var dialogId = Guid.NewGuid().ToString();
		var tcs = new TaskCompletionSource<DIALOGRESULT>();
		_pendingDialogs[dialogId] = tcs;
		_worldActor.ShowDialog(player.Native, dialogId, title, message);
		return await tcs.Task;
	}

}

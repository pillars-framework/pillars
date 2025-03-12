namespace HogWarp.Replicated;

public sealed partial class BpPillarsDialog
{
	public required DialogActor ParentActor { get; set; }
	public partial void DialogResult(NativePlayer player, string dialogId, int result) => ParentActor.DialogResultReceived( dialogId, (DIALOGRESULT)result);
}

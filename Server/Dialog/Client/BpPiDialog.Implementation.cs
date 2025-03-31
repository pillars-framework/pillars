namespace HogWarp.Replicated;

public sealed partial class BpPiDialog
{
	public required DialogActor ParentActor { get; set; }

	/// <summary>
	/// Triggerd by player upon press of a button in a dialog
	/// </summary>
	/// <param name="player">The player pressing the button</param>
	/// <param name="guid">The guid of the dialog</param>
	/// <param name="buttonType">Which button was pressed</param>
	public partial void DialogResponse(NativePlayer player, string guid, int buttonType) =>
		ParentActor.DialogResultReceived(player, guid, buttonType);
}

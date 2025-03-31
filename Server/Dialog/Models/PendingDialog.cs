namespace Pillars.Dialog.Models;

public sealed class PendingDialog
{
	public required string Id { get; init; }
	public ulong PlayerDiscordId { get; init; }
	public required TaskCompletionSource<DIALOGRESULT> Tcs { get; init; }
}

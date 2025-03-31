namespace Pillars.Dialog.Actors;

[RegisterSingleton]
public sealed class DialogActor : PiActor<BpPiDialog>
{
	private readonly ILogger _logger;
	private readonly PlayerController _playerController;

	public DialogActor(ILogger l, PlayerController pc)
	{
		_logger = l.ForThisContext();
		_playerController = pc;
		_worldActor.ParentActor = this;
	}

	#region DIALOG - GENERAL

	public readonly ConcurrentDictionary<string, PendingDialog>
		PendingDialogs = new(); // Maps dialog Ids to pending dialogs

	/// <summary>
	/// Creates a pending dialog for the given player.
	/// </summary>
	/// <param name="player"></param>
	/// <returns>
	/// <c>dialog</c>- if player is valid and dialog successfully created <br/>
	/// <c>null</c>- if the player is invalid or on error
	/// </returns>
	private PendingDialog? CreatePendingDialogForPlayer(PiPlayer player)
	{
		try
		{
			if (player is { IsValid: true, DiscordId: not null })
				return new()
				{
					Id = Guid.NewGuid().ToString(),
					Tcs = new(),
					PlayerDiscordId = (ulong)player.DiscordId
				};
			_logger.Error("{m} - Failed to create pending dialog. Player is not valid anymore.",
				nameof(CreatePendingDialogForPlayer));
			return null;
		}
		catch (Exception ex)
		{
			_logger.Error(ex);
			return null;
		}
	}

	/// <summary>
	/// Triggers a timeout task with the given seconds. Waits until either a result from a client
	/// or the timeout was received. Removes the pending dialog.
	/// </summary>
	/// <param name="dialog">The dialog to await</param>
	/// <param name="timeoutSeconds">The seconds to wait until a timeout should occur</param>
	/// <returns></returns>
	private async Task<DIALOGRESULT> AwaitDialogResult(PendingDialog dialog, long timeoutSeconds)
	{
		try
		{
			Task timeoutTask = Task.Delay(TimeSpan.FromSeconds(timeoutSeconds));
			Task completedTask = await Task.WhenAny(dialog.Tcs.Task, timeoutTask);

			// Remove temporary listener
			PendingDialogs.TryRemove(dialog.Id, out _);

			if (completedTask == timeoutTask)
			{
				_logger.Information(
					"{m} - Received a timeout for dialog #{guid} for player with discordId {pdid}",
					nameof(AwaitDialogResult), dialog.Id, dialog.PlayerDiscordId);
				return DIALOGRESULT.UNDEFINED_TIMEOUT;
			}

			// Check for exceptions in the twdask
			if (dialog.Tcs.Task.Exception != null)
			{
				_logger.Error("{m} - Task-Exception during dialog for player with discordId {pdid}",
					nameof(AwaitDialogResult), dialog.PlayerDiscordId);
				_logger.Error(dialog.Tcs.Task.Exception);
				return DIALOGRESULT.UNDEFINED_TIMEOUT;
			}

			return dialog.Tcs.Task.Result;
		}
		catch (Exception ex)
		{
			_logger.Error(ex);
			return DIALOGRESULT.UNDEFINED_TIMEOUT;
		}
	}

	/// <summary>
	/// Triggered by client actor if a dialog result was received.
	/// Sets the corresponding result
	/// </summary>
	/// <param name="player"></param>
	/// <param name="dialogId"></param>
	/// <param name="dialogResult"></param>
	public void DialogResultReceived(NativePlayer player, string dialogId, int dialogResult)
	{
		try
		{
			if (!_playerController.Players.TryGetValue(player, out var piPlayer))
				return;
			if (!PendingDialogs.TryGetValue(dialogId, out var request))
				return;
			if (request.PlayerDiscordId != piPlayer.DiscordId)
				return;
			PendingDialogs.TryRemove(dialogId, out _);
			request.Tcs.SetResult((DIALOGRESULT)dialogResult);
		}
		catch (Exception ex)
		{
			_logger.Error(ex);
		}
	}

	#endregion

	#region DIALOG

	/// <summary>
	/// Triggers a dialog with a single button
	/// </summary>
	/// <param name="player">The player to show the dialog for</param>
	/// <param name="title">The title of the dialog</param>
	/// <param name="message">The message of the dialog</param>
	/// <param name="okayText">The text on the okay button</param>
	/// <param name="timeoutSeconds">The timeout in seconds.</param>
	/// <returns>Will either return timeout/undefined OR okay</returns>
	public async Task<DIALOGRESULT> ShowOkayDialogAsync(PiPlayer player, string title, string message,
		string okayText = "Okay", long timeoutSeconds = 300)
	{
		try
		{
			// Prepare Dialog
			var dialog = CreatePendingDialogForPlayer(player);
			if (dialog is null || !PendingDialogs.TryAdd(dialog.Id, dialog))
			{
				_logger.Error("{m} - Failed to add a new dialog for player with discordId {pdid}",
					nameof(ShowOkayDialogAsync), player.DiscordId);
				return DIALOGRESULT.UNDEFINED_TIMEOUT;
			}

			// Initiate Dialog
			_worldActor.ShowDialog(player.Native, dialog.Id, (int)DIALOGTYPE.OKAY, title, message, okayText,
				"", "");

			// Await timeout result
			var result = await AwaitDialogResult(dialog, timeoutSeconds);
			return result switch
			{
				DIALOGRESULT.OKAY_YES => result,
				_ => DIALOGRESULT.UNDEFINED_TIMEOUT
			};
		}
		catch (Exception ex)
		{
			_logger.Error(ex);
			return DIALOGRESULT.UNDEFINED_TIMEOUT;
		}
	}

	/// <summary>
	/// Triggers a dialog with two buttons, yes and no
	/// </summary>
	/// <param name="player">The player to show the dialog for</param>
	/// <param name="title">The title of the dialog</param>
	/// <param name="message">The message of the dialog</param>
	/// <param name="yesText">The text on the first button</param>
	/// <param name="noText">The text on the second button</param>
	/// <param name="timeoutSeconds">The timeout in seconds.</param>
	/// <returns>Will either return timeout/undefined OR okay</returns>
	public async Task<DIALOGRESULT> ShowYesNoDialogAsync(PiPlayer player, string title, string message,
		string yesText = "Yes", string noText = "No", long timeoutSeconds = 300)
	{
		try
		{
			// Prepare Dialog
			var dialog = CreatePendingDialogForPlayer(player);
			if (dialog is null || !PendingDialogs.TryAdd(dialog.Id, dialog))
			{
				_logger.Error("{m} - Failed to add a new dialog for player with discordId {pdid}",
					nameof(ShowYesNoDialogAsync), player.DiscordId);
				return DIALOGRESULT.UNDEFINED_TIMEOUT;
			}

			// Initiate Dialog
			_worldActor.ShowDialog(player.Native, dialog.Id, (int)DIALOGTYPE.YES_NO, title, message,
				yesText, noText, "");

			// Await timeout result
			var result = await AwaitDialogResult(dialog, timeoutSeconds);
			return result switch
			{
				DIALOGRESULT.OKAY_YES => result,
				DIALOGRESULT.NO => result,
				_ => DIALOGRESULT.UNDEFINED_TIMEOUT
			};
		}
		catch (Exception ex)
		{
			_logger.Error(ex);
			return DIALOGRESULT.UNDEFINED_TIMEOUT;
		}
	}

	/// <summary>
	/// Triggers a dialog with three buttons, yes, no and cancel.
	/// </summary>
	/// <param name="player">The player to show the dialog for</param>
	/// <param name="title">The title of the dialog</param>
	/// <param name="message">The message of the dialog</param>
	/// <param name="yesText">The text on the first button</param>
	/// <param name="noText">The text on the second button</param>
	/// <param name="cancelText">The text on the third button</param>
	/// <param name="timeoutSeconds">The timeout in seconds.</param>
	/// <returns>Will either return timeout/undefined OR okay</returns>
	public async Task<DIALOGRESULT> ShowYesNoCancelDialogAsync(PiPlayer player, string title, string message,
		string yesText = "Yes", string noText = "No", string cancelText = "Cancel", long timeoutSeconds = 300)
	{
		try
		{
			// Prepare Dialog
			var dialog = CreatePendingDialogForPlayer(player);
			if (dialog is null || !PendingDialogs.TryAdd(dialog.Id, dialog))
			{
				_logger.Error("{m} - Failed to add a new dialog for player with discordId {pdid}",
					nameof(ShowYesNoCancelDialogAsync), player.DiscordId);
				return DIALOGRESULT.UNDEFINED_TIMEOUT;
			}

			// Initiate Dialog
			_worldActor.ShowDialog(player.Native, dialog.Id, (int)DIALOGTYPE.YES_NO_CANCEL, title, message,
				yesText, noText, cancelText);

			// Await timeout result
			var result = await AwaitDialogResult(dialog, timeoutSeconds);
			return result switch
			{
				DIALOGRESULT.OKAY_YES => result,
				DIALOGRESULT.NO => result,
				DIALOGRESULT.CANCEL => result,
				_ => DIALOGRESULT.UNDEFINED_TIMEOUT
			};
		}
		catch (Exception ex)
		{
			_logger.Error(ex);
			return DIALOGRESULT.UNDEFINED_TIMEOUT;
		}
	}

	#endregion
}

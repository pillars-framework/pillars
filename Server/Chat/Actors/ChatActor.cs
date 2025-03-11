namespace Pillars.Chat.Actors;

[RegisterSingleton]
public sealed class ChatActor : PiActor<BpPiChat>
{
	private readonly ILogger _logger;
	private readonly ChatEvents _chatEvents;
	private readonly PlayerController _playerController;
	private readonly ChatService _chatService;

	public ChatActor(ILogger l, ChatEvents ce, PlayerController pc, ChatService cs)
	{
		_logger = l.ForThisContext();
		_chatEvents = ce;
		_playerController = pc;
		_chatService = cs;
		_worldActor.ParentActor = this;
	}

	#region INITIALIZE

	/// <summary>
	/// Initializes the chat for the target with the given chat settings
	/// </summary>
	/// <param name="target">The target player</param>
	/// <param name="settings">The used settings, if null uses default values</param>
	public async Task InitializeChat(PiPlayer target, ChatSettings? settings) =>
		_worldActor.Initialize(target.Native,
			settings?.Height ?? ChatSetting.DEFAULT_HEIGHT,
			settings?.Width ?? ChatSetting.DEFAULT_WIDTH,
			settings?.TextScale ?? ChatSetting.DEFAULT_TEXT_SCALE);

	#endregion

	#region SEND

	/// <summary>
	/// Sends a message to a specific player by forwarding it to the world actor's ReceiveMsg method.
	/// </summary>
	/// <param name="target">The player to whom the message is sent.</param>
	/// <param name="message">The message to send.</param>
	public async Task SendMessage(PiPlayer target, string message) =>
		_worldActor.FromServer(target.Native, message);

	/// <summary>
	/// Sends a message to a set of players by forwarding it to the world actor's ReceiveMsg method.
	/// </summary>
	/// <param name="targets">The players to whom the message is sent.</param>
	/// <param name="message">The message to send.</param>
	public async Task SendMessage(IEnumerable<PiPlayer> targets, string message)
	{
		foreach (var target in targets)
			SendMessage(target, message);
	}

	/// <summary>
	/// Broadcasts a message to all players by forwarding it to the world actor's ReceiveMsg method.
	/// </summary>
	/// <param name="message">The message to send.</param>
	public async Task BroadcastMessage(string message)
	{
		foreach (var player in _playerController.Players.Values)
			SendMessage(player, message);
	}

	#endregion

	#region RECEIVE

	/// <summary>
	/// Handles a received message from a player and forwards it to the chat events system.
	/// </summary>
	/// <param name="player">The player who sent the message.</param>
	/// <param name="message">The message sent by the player.</param>
	public void PlayerMessageReceived(NativePlayer player, string message)
	{
		if (!_playerController.Players.TryGetValue(player, out var piPlayer)) return;
		_chatEvents.ChatMessage(piPlayer, message);
	}

	#endregion

	#region SETTINGS

	/// <summary>
	/// Saves the settings of the player into the database
	/// </summary>
	/// <param name="player">The player initiating the save</param>
	/// <param name="height">The height of the chat panel in px</param>
	/// <param name="width">The width of the chat panel in px</param>
	/// <param name="textScale">The text scale</param>
	public async Task SavePlayerSettings(NativePlayer player, float height, float width, float textScale)
	{
		try
		{
			if (!_playerController.Players.TryGetValue(player, out var piPlayer)) return;
			_logger.Debug("Player #{pid} - saved settings Height: {y}, Width: {x}, fontSize: {fs}",
				player.Id,
				height, width,
				textScale);
			// Retrieve or create settings entity
			var settings = await _chatService.GetSettingsForAccountIdAsync(piPlayer.AccountId) ??
			               new() { Account = new(piPlayer.AccountId) };
			settings.TextScale = textScale;
			settings.Height = height;
			settings.Width = width;

			await settings.SaveAsync();
		}
		catch (Exception ex)
		{
			_logger.Error(ex, ex.Message);
		}
	}

	#endregion
}

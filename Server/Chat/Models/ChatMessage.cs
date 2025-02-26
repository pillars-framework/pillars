namespace Pillars.Core.Player.Models;

/// <summary>
/// This class
/// </summary>
public sealed class ChatMessage
{
	private bool _includeTimestamp;
	private CHATICON? _icon;
	private string _sender = string.Empty;
	private string _texts = string.Empty;

	/// <summary>
	/// Contains the completely builded chat message
	/// </summary>
	public string Message { get; private set; } = string.Empty;

	public sealed class Builder(bool includeTimestamp = true)
	{
		private readonly ChatMessage _chatMessage = new() { _includeTimestamp = includeTimestamp };

		/// <summary>
		/// Prepends the chat message with an icon
		/// </summary>
		public Builder AddIcon(CHATICON icon)
		{
			_chatMessage._icon = icon;
			return this;
		}

		#region SENDER

		/// <summary>
		/// Adds a sender to the chat message with default style
		/// </summary>
		public Builder AddSender(string sender)
		{
			_chatMessage._sender = $"{sender}:";
			return this;
		}

		/// <summary>
		/// Adds a sender to the chat message
		/// </summary>
		public Builder AddSender(string sender, ChatStyle style)
		{
			if (style.Style == CHATTEXTSTYLE.DEFAULT)
				return AddSender(sender);
			_chatMessage._sender += $"<{style.Value}>{sender}:</>";
			return this;
		}

		/// <summary>
		/// Adds a sender to the chat message with the given style
		/// </summary>
		public Builder AddSender(string sender, CHATTEXTSTYLE style) =>
			ChatStyles.Styles.TryGetValue(style, out var textStyle)
				? AddSender(sender, textStyle)
				: AddSender(sender);

		#endregion

		#region TEXTS

		/// <summary>
		/// Adds a default text to the message
		/// </summary>
		public Builder AddText(string text)
		{
			_chatMessage._texts += text;
			return this;
		}

		/// <summary>
		/// Adds a text to the chat message with the given style
		/// </summary>
		public Builder AddText(string text, ChatStyle style)
		{
			if (style.Style == CHATTEXTSTYLE.DEFAULT)
				return AddText(text);
			_chatMessage._texts += $"<{style.Value}>{text}</>";
			return this;
		}

		/// <summary>
		/// Adds a text to the chat message with the given style
		/// </summary>
		public Builder AddText(string text, CHATTEXTSTYLE style) =>
			ChatStyles.Styles.TryGetValue(style, out var textStyle)
				? AddText(text, textStyle)
				: AddText(text);

		/// <summary>
		/// Builds the chat message based on the available icon, sender and texts.
		/// </summary>
		public ChatMessage Build()
		{
			if (_chatMessage._includeTimestamp)
				_chatMessage.Message += DateTime.Now.ToString("[HH:mm] ");
			if (_chatMessage._icon is not null)
				_chatMessage.Message += $"<img id=\"{_chatMessage._icon}\"></> ";
			if (!string.IsNullOrEmpty(_chatMessage._sender))
				_chatMessage.Message += $"{_chatMessage._sender} "; // Just for the whitespace
			_chatMessage.Message += _chatMessage._texts;
			return _chatMessage;
		}

		#endregion
	}
}

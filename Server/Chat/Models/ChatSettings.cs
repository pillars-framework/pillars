namespace Pillars.Entities;

/// <summary>
/// The chat settings database entity, saved on a "per-account" basis
/// </summary>
[Collection("chat_settings")]
public sealed class ChatSettings : Entity
{
	/// <summary>
	/// The account that use this settings
	/// </summary>
	public required One<Account> Account { get; set; }

	/// <summary>
	/// The height of the chat in px
	/// </summary>
	public float Height { get; set; } = ChatSetting.DEFAULT_HEIGHT;

	/// <summary>
	/// The width of the chat in px
	/// </summary>
	public float Width { get; set; } = ChatSetting.DEFAULT_WIDTH;

	/// <summary>
	/// Scaling of the text
	/// </summary>
	public float TextScale { get; set; } = ChatSetting.DEFAULT_TEXT_SCALE;
}

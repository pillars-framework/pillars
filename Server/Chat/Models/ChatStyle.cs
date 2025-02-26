namespace Pillars.Core.Player.Models;

/// <summary>
/// Class representing a possible chat style
/// </summary>
/// <param name="style">The enum for this style</param>
/// <param name="value">The value for this style, used in the final message</param>
public sealed class ChatStyle(CHATTEXTSTYLE style, string value)
{
	public readonly CHATTEXTSTYLE Style = style;
	public readonly string Value = value;
}

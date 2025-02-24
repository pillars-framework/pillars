namespace Pillars.Chat.Attributes;

/// <summary>
/// Marks a method as a slash command, associating it with a unique identifier.
/// </summary>
/// <remarks>
/// This attribute is used to define methods that handle slash commands in the application.
/// The <see cref="Identifier"/> property specifies the command's unique name, which is used to invoke the command.
///
/// The attribute can only be applied to methods and does not support multiple instances or inheritance.
/// </remarks>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class SlashCommandAttribute(string identifier) : Attribute
{
	public string Identifier { get; set; } = identifier;

}

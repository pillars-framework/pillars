namespace Pillars.Entities;

public sealed partial class Account
{
	/// <summary>
	/// Gets or sets a value indicating whether the player or user is allowed to send messages.
	/// </summary>
	/// <value>
	/// <c>true</c> if the player or user is allowed to send messages; otherwise, <c>false</c>.
	/// </value>
	public bool CanSendMessages { get; set; }
}

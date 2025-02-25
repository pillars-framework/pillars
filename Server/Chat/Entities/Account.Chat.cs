namespace Pillars.Entities;

public sealed partial class Account
{
	/// <summary>
	/// Flag to indicate if the account can speak parseltongue.
	/// This also means that the player of the account can understand parseltongue.
	/// </summary>
	public bool CanSpeakParsel { get; set; }
}

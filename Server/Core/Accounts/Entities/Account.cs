namespace Pillars.Entities;

[Collection("accounts")]
public sealed partial class Account : Entity, ICreatedOn, IModifiedOn
{
	public ulong DiscordId { get; set; }

	public DateTime CreatedOn { get; set; }

	public DateTime ModifiedOn { get; set; }
}

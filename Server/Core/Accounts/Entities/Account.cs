namespace Pillars.Entities;

[Collection("accounts")]
public sealed partial class Account : Entity, ICreatedOn, IModifiedOn
{
	public ulong DiscordId { get; set; }

	public uint OpenCharSlots { get; set; } = 1;

	public DateTime LastLogin { get; set; }

	public bool Banned { get; set; }

	public DateTime? LockedUntil { get; set;  }

	public bool IsTemporaryLocked => LockedUntil is not null &&
		DateTime.Compare((DateTime)LockedUntil, DateTime.UtcNow) > 0;

	public DateTime CreatedOn { get; set; }

	public DateTime ModifiedOn { get; set; }
}

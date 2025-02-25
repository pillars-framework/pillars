namespace Pillars.Core.Player.Models;

public sealed partial class PiPlayer
{
	public NativePlayer Native { get; set; } = native;
	public IPlayer InternalPlayer => Native.InternalPlayer;
	public void Kick() => Native.Kick();
	public ulong Id => Native.Id;
	public uint ConnectionId => Native.ConnectionId;
	public string UniqueId => Native.UniqueId;
	public string Username => Native.Username;
	public FVector Position => Native.Position;
	public float Direction => Native.Direction;
	public float Speed => Native.Speed;
	public bool IsInAir => Native.IsInAir;
	public byte House => Native.House;
	public byte Gender => Native.Gender;
	public bool IsMounted => Native.IsMounted;
}

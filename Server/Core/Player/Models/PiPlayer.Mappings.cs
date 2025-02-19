namespace Pillars.Core.Player.Models;

public sealed partial class PiPlayer
{
    public IPlayer InternalPlayer => Player.InternalPlayer;
    public void Kick() => Player.Kick();

    public ulong Id => Player.Id;

    public uint ConnectionId => Player.ConnectionId;

    public string UniqueId => Player.UniqueId;

    public string Username => Player.Username;

    public FVector Position => Player.Position;

    public float Direction => Player.Direction;

    public float Speed => Player.Speed;

    public bool IsInAir => Player.IsInAir;

    public byte House => Player.House;

    public byte Gender => Player.Gender;

    public bool IsMounted => Player.IsMounted;
}

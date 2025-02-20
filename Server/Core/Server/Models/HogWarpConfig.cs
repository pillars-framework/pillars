namespace Pillars.Core.Server.Models;

/// <summary>
/// An init-only config model representing the current server config, read
/// from the existing json file.
/// </summary>
public sealed class HogWarpConfig
{
	public string ApiKey { get; init; } = string.Empty;
	public string Description { get; init; } = string.Empty;
	public bool Developer { get; init; }
	public string IconUrl { get; init; } = string.Empty;
	public int MaxPlayer { get; init; }
	public string Name { get; init; } = string.Empty;
	public string Password { get; init; } = string.Empty;
	public int Port { get; init; }
	public bool Public { get; init; }
	public string Tags { get; init; } = string.Empty;
	public int TickRate { get; init; }
}

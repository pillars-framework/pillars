namespace Pillars.Core.Database.Configuration;

/// <summary>
/// Credential class for the database.
/// </summary>
public sealed class DatabaseCredentials
{
	public string User { get; set; } = string.Empty;
	public string Password { get; set; } = string.Empty;
}

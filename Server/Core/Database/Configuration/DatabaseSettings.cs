namespace Pillars.Core.Database.Configuration;

/// <summary>
/// DatabaseSettings loaded from mongosettings.json
/// </summary>
public sealed class DatabaseSettings : ISettings
{
	/// <summary>
	/// Name of the database, defaults to "hogwarp"
	/// </summary>
	public string Database { get; set; } = "hogwarp";

	/// <summary>
	/// Database host, defaults to "localhost"
	/// </summary>
	public string Host { get; set; } = "localhost";

	/// <summary>
	/// Database Port, defaults to 27017
	/// </summary>
	public int Port { get; set; } = 27017;

	/// <summary>
	/// The (optional) credentials used for the database.
	/// </summary>
	/// <remarks>
	/// Optional. If not provided, no credentials will be used.
	/// Only applicable if MongoDB is installed as a service.
	/// </remarks>
	public DatabaseCredentials? Credentials { get; set; }

	/// <summary>
	/// If enabled, logs database commands as debug level.
	/// Defaults to "false"
	/// </summary>
	public bool DebugLog { get; set; }
}

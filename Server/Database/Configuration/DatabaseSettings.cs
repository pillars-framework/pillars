namespace Pillars.Database.Configuration;

public sealed class DatabaseSettings : ISettings
{
    public string Database { get; set; } = default!;
    public string Host { get; set; } = default!;
    public int Port { get; set; } = 27017;
}

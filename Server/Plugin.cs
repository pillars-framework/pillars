namespace Pillars;

public sealed class Plugin : IPlugin
{
    public string Author { get; } = "Pillars Team";
    public string Name { get; } = "Pillars Framework";
    public Version Version { get; } = new(1, 0, 0);

    public Plugin()
    {
        Task.Run(async () =>
        {
            try
            {
                await new Bootstrapper().RunAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        });
    }

    public void PostLoad() {}
    public void Shutdown() {}
}

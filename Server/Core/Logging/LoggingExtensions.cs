namespace Pillars.Core.Logging;

public static partial class LoggingExtensions
{
    public static void Error(this ILogger logger, Exception exception) => logger.Error(exception, "{Message}", "SCRIPTERROR");

    /// <summary>
    /// Creates a logger for the type the method is invoked in.
    /// </summary>
    /// <param name="logger">The logger where the context is mapped to</param>
    /// <param name="filePath">DO NOT PASS ANYTHING</param>
    /// <returns>The passed logger</returns>
    public static ILogger ForThisContext(this ILogger logger, [CallerFilePath] string filePath = "")
    {
        var className = Path.GetFileNameWithoutExtension(filePath);
        var type = Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(t => t.Name == className);
        return type != null ? logger.ForContext(type) : logger;
    }
}

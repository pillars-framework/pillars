namespace Pillars.Core.Logging;

public static class ConsoleThemes
{
    public static AnsiConsoleTheme Console { get; } = new(
        new Dictionary<ConsoleThemeStyle, string>
        {
            [ConsoleThemeStyle.Text] = "\x1b[38;5;0015m",
            [ConsoleThemeStyle.SecondaryText] = "\x1b[38;5;141m",
            [ConsoleThemeStyle.TertiaryText] = "\x1b[38;5;7m",
            [ConsoleThemeStyle.Invalid] = "\x1b[38;5;211m",
            [ConsoleThemeStyle.Null] = "\x1b[38;5;9m",
            [ConsoleThemeStyle.Name] = "\x1b[38;5;211m",
            [ConsoleThemeStyle.String] = "\x1b[38;5;74m",
            [ConsoleThemeStyle.Number] = "\x1b[38;5;211m",
            [ConsoleThemeStyle.Boolean] = "\x1b[38;5;26m",
            [ConsoleThemeStyle.Scalar] = "\x1b[38;5;211m",
            [ConsoleThemeStyle.LevelVerbose] = "\x1b[38;5;214m",
            [ConsoleThemeStyle.LevelDebug] = "\x1b[38;5;214m",
            [ConsoleThemeStyle.LevelInformation] = "\x1b[38;5;10m",
            [ConsoleThemeStyle.LevelWarning] = "\x1b[38;5;0011m",
            [ConsoleThemeStyle.LevelError] = "\x1b[38;5;196m",
            [ConsoleThemeStyle.LevelFatal] = "\x1b[38;5;196m"
        });
}

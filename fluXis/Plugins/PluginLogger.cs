using System;
using osu.Framework.Logging;

namespace fluXis.Plugins;

public class PluginLogger
{
    private readonly string prefix;
    private static Logger logger { get; } = Logger.GetLogger("plugins");

    internal PluginLogger(string pluginName)
    {
        prefix = $"[{pluginName}]";
    }

    public void Log(string message) => log(message, LogLevel.Verbose);
    public void Info(string message) => log(message, LogLevel.Important);
    public void Error(string message, Exception e = null) => logError(message, e);

    private void log(string message, LogLevel level)
    {
        logger.Add($"{prefix} {message}", level);
    }

    private void logError(string message, Exception e = null)
    {
        logger.Add($"{prefix} {message}", LogLevel.Error, e);
    }
}

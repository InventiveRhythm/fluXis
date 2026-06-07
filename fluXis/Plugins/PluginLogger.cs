using System;
using System.Collections.Concurrent;
using System.Reflection;
using osu.Framework.Logging;

namespace fluXis.Plugins;

public static class PluginLogger
{
    private static Logger logger { get; } = Logger.GetLogger("plugins");
    private static readonly ConcurrentDictionary<Assembly, string> plugin_names = new();

    internal static void RegisterPlugin(Assembly assembly, string pluginName)
    {
        plugin_names.TryAdd(assembly, pluginName);
    }

    public static void Log(string message)
        => log(message, LogLevel.Verbose, getPluginName(Assembly.GetCallingAssembly()));

    public static void Info(string message)
        => log(message, LogLevel.Important, getPluginName(Assembly.GetCallingAssembly()));

    public static void Error(string message, Exception e = null)
        => logError(message, e, getPluginName(Assembly.GetCallingAssembly()));

    private static string getPluginName(Assembly caller)
    {
        return plugin_names.TryGetValue(caller, out var name) ? name : caller.GetName().Name;
    }

    private static void log(string message, LogLevel level, string pluginName)
    {
        logger.Add($"[{pluginName}] {message}", level);
    }

    private static void logError(string message, Exception e, string pluginName)
    {
        logger.Add($"[{pluginName}] {message}", LogLevel.Error, e);
    }
}

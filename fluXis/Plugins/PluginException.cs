using System;

namespace fluXis.Plugins;

public class PluginException : Exception
{
    public PluginException(string message)
        : base($"Plugin Error: {message}")
    {
    }

    public PluginException(string message, Exception e)
        : base($"Plugin Error: {message}", e)
    {
    }
}

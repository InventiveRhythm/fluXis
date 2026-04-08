using System.Collections.Generic;
using System.Linq;
using fluXis.Plugins;

namespace fluXis.Utils.Extensions;

public static class PluginExtensions
{
    /// <summary>
    /// Get all capabilities of a type across all plugins
    /// </summary>
    public static IEnumerable<T> GetCapabilities<T>(this IEnumerable<Plugin> plugins)
        where T : class, IPluginCapability
        => plugins.Select(p => p.GetCapability<T>())
                  .Where(c => c != null);

    /// <summary>
    /// Get all (plugin, capability) pairs across all plugins
    /// </summary>
    public static IEnumerable<(Plugin Plugin, T Capability)> GetCapabilityPairs<T>(this IEnumerable<Plugin> plugins)
        where T : class, IPluginCapability
        => plugins.SelectMany(p => p.GetCapabilities<T>()
                                    .Select(c => (Plugin: p, Capability: c)));
}

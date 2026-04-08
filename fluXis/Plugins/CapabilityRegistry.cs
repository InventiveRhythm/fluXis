using System;
using System.Collections.Generic;
using osu.Framework.Logging;

namespace fluXis.Plugins;

internal static class CapabilityRegistry
{
    private static readonly Dictionary<Type, object> providers = new();

    internal static void Register<T>(T provider, bool log = true) where T : class
    {
        providers[typeof(T)] = provider;
        if (log) Logger.Log($"{typeof(T).Name} registered for plugins.");
    }

    internal static T Get<T>() where T : class
    {
        if (providers.TryGetValue(typeof(T), out var provider))
            return (T)provider;

        throw new KeyNotFoundException($"{typeof(T).Name} not found in providers, providers available: {string.Join(", ", providers.Keys)}.");
    }
}

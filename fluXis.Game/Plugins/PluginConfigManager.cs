using System;
using osu.Framework.Configuration;
using osu.Framework.Platform;

namespace fluXis.Game.Plugins;

public abstract class PluginConfigManager<TLookup> : IniConfigManager<TLookup>
    where TLookup : struct, Enum
{
    protected abstract string ID { get; }
    protected sealed override string Filename => $"{ID}.config.ini";

    protected PluginConfigManager(Storage storage)
        : base(storage)
    {
    }
}

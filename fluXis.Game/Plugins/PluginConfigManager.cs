using System;
using osu.Framework.Configuration;
using osu.Framework.Platform;

namespace fluXis.Game.Plugins;

public class PluginConfigManager<TLookup> : IniConfigManager<TLookup>
    where TLookup : struct, Enum
{
    protected sealed override string Filename { get; }

    public PluginConfigManager(string id, Storage storage)
        : base(storage)
    {
        Filename = $"{id}.config.ini";
    }
}

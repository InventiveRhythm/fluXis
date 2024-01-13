using System;
using System.IO;
using fluXis.Game.Plugins;
using osu.Framework.Platform;

namespace fluXis.Import.osu;

public class OsuPluginConfig : PluginConfigManager<OsuPluginSetting>
{
    public OsuPluginConfig(Storage storage)
        : base("osu", storage)
    {
    }

    protected override void InitialiseDefaults()
    {
        base.InitialiseDefaults();

        SetDefault(OsuPluginSetting.GameLocation, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "osu!"));
    }
}

public enum OsuPluginSetting
{
    GameLocation
}

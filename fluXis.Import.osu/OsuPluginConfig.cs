using System;
using System.IO;
using fluXis.Plugins;
using osu.Framework.Platform;

namespace fluXis.Import.osu;

public class OsuPluginConfig : PluginConfigManager<OsuPluginSetting>
{
    protected override string ID => "osu";

    public OsuPluginConfig(Storage storage)
        : base(storage)
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

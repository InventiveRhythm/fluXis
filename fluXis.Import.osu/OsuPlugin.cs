using System;
using System.Collections.Generic;
using fluXis.Overlay.Settings.UI;
using fluXis.Plugins;
using osu.Framework.Platform;

namespace fluXis.Import.osu;

public class OsuPlugin : Plugin
{
    public override string Name => "osu! Importer";
    public override string Author => "Flustix";
    public override Version Version => new(1, 2, 0);

    private OsuPluginConfig config;

    protected override MapImporter CreateImporter() => new OsuImport(config);
    public override void CreateConfig(Storage storage) => config = new OsuPluginConfig(storage);

    public override List<SettingsItem> CreateSettings() => new()
    {
        new SettingsToggle
        {
            Label = "Skip Backgrounds",
            Description = "Skips fetching the background from the map file, speeding up importing by a lot.",
            Bindable = config.GetBindable<bool>(OsuPluginSetting.SkipBackgrounds)
        },
        new SettingsTextBox
        {
            Label = "osu! Directory",
            Description = "The directory where osu! is installed.",
            Bindable = config.GetBindable<string>(OsuPluginSetting.GameLocation)
        }
    };
}

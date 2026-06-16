using System;
using System.Collections.Generic;
using fluXis.Overlay.Settings.UI;
using fluXis.Plugins;
using fluXis.Plugins.Capabilities;
using osu.Framework.Platform;

namespace fluXis.Import.Stepmania;

public class StepmaniaPlugin : Plugin, IMapImporterCapability
{
    public override string Name => "Stepmania Importer";
    public override string Author => "Flustix";
    public override Version Version => new(1, 2, 0);
    public override PluginType Type => PluginType.Import;

    private StepmaniaPluginConfig config;

    private MapImporter importer;
    public MapImporter Importer => importer ??= new StepmaniaImport(config);

    public override void CreateConfig(Storage storage) => config = new StepmaniaPluginConfig(storage);

    public override List<SettingsItem> CreateSettings() => new()
    {
        new SettingsTextBox
        {
            Label = "StepMania Song Directory",
            Description = "The directory where your StepMania songs are located.",
            Bindable = config.GetBindable<string>(StepmaniaPluginSetting.GameLocation)
        }
    };
}

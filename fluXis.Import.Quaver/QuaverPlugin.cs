using System;
using System.Collections.Generic;
using fluXis.Overlay.Settings.UI;
using fluXis.Plugins;
using fluXis.Plugins.Capabilities;
using osu.Framework.Platform;

namespace fluXis.Import.Quaver;

public class QuaverPlugin : Plugin, IMapImporterCapability
{
    public override string Name => "Quaver Importer";
    public override string Author => "Flustix";
    public override Version Version => new(1, 3, 0);
    public override PluginType Type => PluginType.Import;

    private QuaverPluginConfig config;

    private MapImporter importer;
    public MapImporter Importer => importer ??= new QuaverImport(config);

    public override void CreateConfig(Storage storage) => config = new QuaverPluginConfig(storage);

    public override List<SettingsItem> CreateSettings() => new()
    {
        new SettingsTextBox
        {
            Label = "Quaver Directory",
            Description = "The directory where Quaver is installed.",
            Bindable = config.GetBindable<string>(QuaverPluginSetting.GameLocation)
        }
    };
}

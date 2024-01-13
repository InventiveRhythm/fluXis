using System;
using fluXis.Game.Import;
using fluXis.Game.Plugins;
using osu.Framework.Platform;

namespace fluXis.Import.Quaver;

public class QuaverPlugin : Plugin
{
    public override string Name => "Quaver Importer";
    public override string Author => "Flustix";
    public override Version Version => new(1, 2, 0);

    private QuaverPluginConfig config;

    protected override MapImporter CreateImporter() => new QuaverImport(config);
    public override void CreateConfig(Storage storage) => config = new QuaverPluginConfig(storage);
}

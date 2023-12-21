using System;
using fluXis.Game.Import;
using fluXis.Game.Plugins;

namespace fluXis.Import.Quaver;

public class QuaverPlugin : Plugin
{
    public override string Name => "Quaver Importer";
    public override string Author => "Flustix";
    public override Version Version => new(1, 2, 0);

    protected override MapImporter CreateImporter() => new QuaverImport();
}

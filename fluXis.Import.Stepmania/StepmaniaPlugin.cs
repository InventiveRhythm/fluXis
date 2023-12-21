using System;
using fluXis.Game.Import;
using fluXis.Game.Plugins;

namespace fluXis.Import.Stepmania;

public class StepmaniaPlugin : Plugin
{
    public override string Name => "Stepmania Importer";
    public override string Author => "Flustix";
    public override Version Version => new(1, 1, 0);

    protected override MapImporter CreateImporter() => new StepmaniaImport();
}

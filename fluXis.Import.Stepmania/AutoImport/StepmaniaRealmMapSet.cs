using System.Collections.Generic;
using fluXis.Game.Database.Maps;

namespace fluXis.Import.Stepmania.AutoImport;

public class StepmaniaRealmMapSet : RealmMapSet
{
    public string FolderPath { get; init; } = string.Empty;

    public StepmaniaRealmMapSet(List<RealmMap> maps)
        : base(maps)
    {
    }

    public override string GetPathForFile(string filename) => $"{FolderPath}/{filename}";
}

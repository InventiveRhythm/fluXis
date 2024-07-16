using System.Collections.Generic;
using System.IO;
using fluXis.Game.Database.Maps;

namespace fluXis.Import.Stepmania.AutoImport;

public class StepmaniaRealmMapSet : RealmMapSet
{
    public string FolderPath { get; init; } = string.Empty;

    public StepmaniaRealmMapSet(List<RealmMap> maps)
        : base(maps)
    {
    }

    public override string GetPathForFile(string filename)
        => string.IsNullOrEmpty(filename) ? FolderPath : Path.Combine(FolderPath, filename);
}

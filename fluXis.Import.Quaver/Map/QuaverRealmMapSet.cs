using System.Collections.Generic;
using System.IO;
using fluXis.Database.Maps;

namespace fluXis.Import.Quaver.Map;

public class QuaverRealmMapSet : RealmMapSet
{
    public string FolderPath { get; init; } = string.Empty;

    public QuaverRealmMapSet(List<RealmMap> maps)
        : base(maps)
    {
    }

    public override string GetPathForFile(string filename)
        => string.IsNullOrEmpty(filename) ? FolderPath : Path.Combine(FolderPath, filename);
}

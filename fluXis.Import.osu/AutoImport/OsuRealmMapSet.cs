using System.Collections.Generic;
using System.IO;
using fluXis.Game.Database.Maps;

namespace fluXis.Import.osu.AutoImport;

public class OsuRealmMapSet : RealmMapSet
{
    public string FolderPath { get; init; } = string.Empty;

    public OsuRealmMapSet(List<RealmMap> maps)
        : base(maps)
    {
    }

    public override string GetPathForFile(string filename)
        => string.IsNullOrEmpty(filename) ? FolderPath : Path.Combine(FolderPath, filename);
}

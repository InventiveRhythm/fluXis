using System.IO;
using fluXis.Database.Maps;
using fluXis.Map;

namespace fluXis.Import.osu.AutoImport;

public class OsuRealmMap : RealmMap
{
    public override MapInfo GetMapInfo()
    {
        var path = MapSet.GetPathForFile(FileName);

        if (!File.Exists(path))
            return null;

        string osu = File.ReadAllText(path);
        var info = OsuImport.ParseOsuMap(osu).ToMapInfo();

        if (info is null)
            return null;

        info.Map = this;
        return info;
    }
}

using System.IO;
using fluXis.Game.Database.Maps;
using fluXis.Game.Map;

namespace fluXis.Import.osu.AutoImport;

public class OsuRealmMap : RealmMap
{
    public override MapInfo GetMapInfo()
    {
        var path = MapSet.GetPathForFile(FileName);
        string osu = File.ReadAllText(path);
        return OsuImport.ParseOsuMap(osu).ToMapInfo();
    }
}

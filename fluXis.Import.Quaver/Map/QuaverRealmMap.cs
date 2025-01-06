using System.IO;
using fluXis.Database.Maps;
using fluXis.Map;

namespace fluXis.Import.Quaver.Map;

public class QuaverRealmMap : RealmMap
{
    public override MapInfo GetMapInfo()
    {
        var path = MapSet.GetPathForFile(FileName);

        if (!File.Exists(path))
            return null;

        string yaml = File.ReadAllText(path);
        var map = QuaverImport.ParseFromYaml(yaml).ToMapInfo();
        map.RealmEntry = this;
        return map;
    }
}

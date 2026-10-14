using System.IO;
using fluXis.Database.Maps;
using fluXis.Map;
using fluXis.Modes;

namespace fluXis.Import.Quaver.Map;

public class QuaverRealmMap : RealmMap
{
    public override PlayableMap GetPlayable(GameModeManager modes)
    {
        var path = MapSet.GetPathForFile(FileName);

        if (!File.Exists(path))
            return null;

        string yaml = File.ReadAllText(path);
        var map = QuaverImport.ParseFromYaml(yaml).ToPlayableMap();
        // map.RealmEntry = this;
        return map;
    }
}

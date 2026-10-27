using System.IO;
using fluXis.Database.Maps;
using fluXis.Map;
using fluXis.Modes;
using osu.Framework.Platform;

namespace fluXis.Import.Quaver.Map;

public class QuaverRealmMap : RealmMap
{
    public override PlayableMap GetPlayable(GameModeManager modes)
    {
        var path = MapSet.GetPathForFile(FileName);

        if (!File.Exists(path))
            return null;

        var dir = Path.GetDirectoryName(path);
        var storage = new NativeStorage(dir);
        string yaml = File.ReadAllText(path);
        var map = QuaverImport.ParseFromYaml(yaml).ToPlayableMap(storage);
        return map;
    }
}

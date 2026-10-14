using System.IO;
using fluXis.Database.Maps;
using fluXis.Map;
using fluXis.Modes;

namespace fluXis.Import.osu.AutoImport;

public class OsuRealmMap : RealmMap
{
    public override PlayableMap GetPlayable(GameModeManager modes)
    {
        var path = MapSet.GetPathForFile(FileName);

        if (!File.Exists(path))
            return null;

        var dir = Path.GetDirectoryName(path);
        var files = Directory.GetFiles(dir!, "*", SearchOption.AllDirectories);

        string osu = File.ReadAllText(path);
        var info = OsuImport.ParseOsuMap(osu, files).ToPlayableMap();

        if (info is null)
            return null;

        Metadata.Background = info.BackgroundFile;
        // info.RealmEntry = this;
        return info;
    }
}

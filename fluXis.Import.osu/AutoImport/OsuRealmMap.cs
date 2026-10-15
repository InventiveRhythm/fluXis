using System.IO;
using fluXis.Database.Maps;
using fluXis.Map;
using fluXis.Modes;
using osu.Framework.Platform;

namespace fluXis.Import.osu.AutoImport;

public class OsuRealmMap : RealmMap
{
    public override PlayableMap GetPlayable(GameModeManager modes)
    {
        var path = MapSet.GetPathForFile(FileName);

        if (!File.Exists(path))
            return null;

        var dir = Path.GetDirectoryName(path);
        var storage = new NativeStorage(dir);
        var playable = OsuImport.ParseOsuMap(storage, FileName).ToPlayableMap();

        if (playable is null)
            return null;

        Metadata.Background = playable.BackgroundFile;
        return playable;
    }
}

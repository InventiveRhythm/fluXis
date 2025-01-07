using System.IO;
using fluXis.Map;

namespace fluXis.Import.osu.AutoImport;

public class OsuMapInfo : MapInfo
{
    public override Stream GetVideoStream()
    {
        var path = RealmEntry!.MapSet.GetPathForFile(VideoFile);

        if (string.IsNullOrEmpty(path) || !File.Exists(path))
            return null;

        return File.OpenRead(path);
    }
}

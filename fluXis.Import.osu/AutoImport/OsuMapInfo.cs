using System.IO;
using System.Text.Json.Serialization;
using fluXis.Map;

namespace fluXis.Import.osu.AutoImport;

public class OsuMapInfo : MapInfo
{
    [JsonIgnore]
    public new OsuRealmMap Map { get; set; }

    public override Stream GetVideoStream()
    {
        var path = Map.MapSet.GetPathForFile(VideoFile);

        if (string.IsNullOrEmpty(path) || !File.Exists(path))
            return null;

        return File.OpenRead(path);
    }
}

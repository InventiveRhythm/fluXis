using System.IO;
using fluXis.Database.Maps;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics.Textures;
using Realms;

namespace fluXis.Map.Builtin.Roundhouse;

[Ignored]
public class RoundhouseMap : RealmMap
{
    public RoundhouseMap()
    {
        ID = default;
        Hash = "dummy";
        Filters = new RealmMapFilters();
        Metadata = new RealmMapMetadata
        {
            Title = "Roundhouse",
            Artist = "Akiri"
        };
    }

    public override MapInfo GetMapInfo() => null;
    public override Texture GetBackground() => null;
    public override Stream GetBackgroundStream() => null;

    public override Track GetTrack()
    {
        var tracks = MapSet.Resources?.TrackStore;
        var track = tracks?.Get("Menu/Roundhouse");

        if (track is not null)
        {
            track.RestartPoint = 1443;
            track.Looping = true;
        }

        return track;
    }
}

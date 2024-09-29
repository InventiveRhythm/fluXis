using System.IO;
using fluXis.Game.Database.Maps;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics.Textures;
using Realms;

namespace fluXis.Game.Map.Builtin.Spoophouse;

[Ignored]
public class SpoophouseMap : RealmMap
{
    public SpoophouseMap()
    {
        ID = default;
        Hash = "dummy";
        Filters = new RealmMapFilters();
        Metadata = new RealmMapMetadata
        {
            Title = "Spoophouse",
            Artist = "Akiri"
        };
    }

    public override MapInfo GetMapInfo() => null;
    public override Texture GetBackground() => null;
    public override Stream GetBackgroundStream() => null;

    public override Track GetTrack()
    {
        var tracks = MapSet.Resources?.TrackStore;
        return tracks?.Get("Menu/Spoophouse.ogg");
    }
}

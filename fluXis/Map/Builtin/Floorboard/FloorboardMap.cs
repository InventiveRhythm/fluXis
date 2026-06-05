using System.IO;
using fluXis.Database.Maps;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics.Textures;
using Realms;

namespace fluXis.Map.Builtin.Floorboard;

[Ignored]
public class FloorboardMap : RealmMap
{
    public FloorboardMap()
    {
        ID = default;
        Hash = "dummy";
        Filters = new RealmMapFilters();
        Metadata = new RealmMapMetadata
        {
            Title = "An Appetising Floorboard",
            Artist = "Akiri"
        };
    }

    public override MapInfo GetMapInfo() => null;
    public override Texture GetBackground() => null;
    public override Stream GetBackgroundStream() => null;

    public override Track GetTrack()
    {
        var tracks = MapSet.Resources?.TrackStore;
        var track = tracks?.Get("Menu/An Appetising Floorboard");
        
        if (track is not null)
        {
            track.Looping = true;
        }
        
        return track;
    }
}

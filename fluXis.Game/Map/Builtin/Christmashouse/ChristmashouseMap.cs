using System.IO;
using fluXis.Game.Database.Maps;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics.Textures;
using Realms;

namespace fluXis.Game.Map.Builtin.Christmashouse;

[Ignored]
public class ChristmashouseMap : RealmMap
{
    public ChristmashouseMap()
    {
        ID = default;
        Hash = "dummy";
        Filters = new RealmMapFilters();
        Metadata = new RealmMapMetadata
        {
            Title = "Christmashouse",
            Artist = "Akiri"
        };
    }

    public override MapInfo GetMapInfo() => null;
    public override Texture GetBackground() => null;
    public override Stream GetBackgroundStream() => null;

    public override Track GetTrack()
    {
        var tracks = MapSet.Resources?.TrackStore;
        return tracks?.Get("Menu/Christmashouse.ogg");
    }
}

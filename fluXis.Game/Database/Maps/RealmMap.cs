using System;
using System.Collections.Generic;
using System.IO;
using fluXis.Game.Map;
using fluXis.Game.Utils;
using JetBrains.Annotations;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics.Textures;
using osu.Framework.Logging;
using Realms;

namespace fluXis.Game.Database.Maps;

public class RealmMap : RealmObject
{
    [PrimaryKey]
    public Guid ID { get; set; }

    public string Difficulty { get; set; } = string.Empty;
    public RealmMapMetadata Metadata { get; set; } = null!;
    public RealmMapSet MapSet { get; set; } = null!;

    /**
     * -2 = Local
     * -1 = Blacklisted (server side)
     * 0 = Unsubmitted
     * 1 = Pending
     * 2 = Impure
     * 3 = Pure
     * everything over 100 is from other games
     */
    public int Status { get; set; } = -2;

    public string FileName { get; set; } = string.Empty;
    public int OnlineID { get; set; } = -1;
    public string Hash { get; set; } = string.Empty;
    public RealmMapFilters Filters { get; set; } = null!;
    public int KeyCount { get; set; } = 4;
    public float Rating { get; set; }

    public RealmMap([CanBeNull] RealmMapMetadata meta = null)
    {
        Metadata = meta ?? new RealmMapMetadata();
        ID = Guid.NewGuid();
    }

    [UsedImplicitly]
    private RealmMap()
    {
    }

    public override string ToString() => $"{ID} - {Metadata}";

    [CanBeNull]
    public virtual MapInfo GetMapInfo()
    {
        try
        {
            var path = MapFiles.GetFullPath(MapSet.GetPathForFile(FileName));
            var json = File.ReadAllText(path);
            var hash = MapUtils.GetHash(json);
            var map = json.Deserialize<MapInfo>();
            map.Map = this;
            map.Hash = hash;
            return map;
        }
        catch (Exception e)
        {
            Logger.Error(e, "Failed to load map from path: " + MapSet.GetPathForFile(FileName));
            return null;
        }
    }

    public virtual Texture GetBackground()
    {
        var backgrounds = MapSet.Resources?.BackgroundStore;
        return backgrounds?.Get(MapSet.GetPathForFile(Metadata.Background));
    }

    public virtual Stream GetBackgroundStream()
    {
        var backgrounds = MapSet.Resources?.BackgroundStore;
        return backgrounds?.GetStream(MapSet.GetPathForFile(Metadata.Background));
    }

    public virtual Texture GetPanelBackground()
    {
        var croppedBackgrounds = MapSet.Resources?.CroppedBackgroundStore;
        return croppedBackgrounds?.Get(MapSet.GetPathForFile(Metadata.Background));
    }

    public virtual Track GetTrack()
    {
        var tracks = MapSet.Resources?.TrackStore;
        return tracks?.Get(MapSet.GetPathForFile(Metadata.Audio));
    }

    public static RealmMap CreateNew()
    {
        RealmMap map = new RealmMap
        {
            Metadata = new RealmMapMetadata(),
            ID = Guid.NewGuid()
        };
        RealmMapSet set = new RealmMapSet(new List<RealmMap> { map });
        map.MapSet = set;
        return map;
    }
}

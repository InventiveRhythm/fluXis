using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Map;
using JetBrains.Annotations;
using Newtonsoft.Json;
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

    public int OnlineID { get; set; } = -1;
    public string Hash { get; set; } = string.Empty;
    public RealmMapFilters Filters { get; set; } = null!;
    public int KeyCount { get; set; } = 4;
    public float Rating { get; set; }

    [Ignored]
    public RealmFile File => MapSet.Files.FirstOrDefault(f => f.Hash == Hash);

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
            var path = RealmStorage.GetFullPath(File);
            var json = System.IO.File.ReadAllText(path);
            MapInfo map = JsonConvert.DeserializeObject<MapInfo>(json);
            map.Map = this;
            return map;
        }
        catch (Exception e)
        {
            Logger.Error(e, "Failed to load map from path: " + File.Path);
            return null;
        }
    }

    public virtual Texture GetBackground()
    {
        var backgrounds = MapSet.Resources?.BackgroundStore;
        if (backgrounds == null) return null;

        var file = MapSet.GetFile(Metadata.Background);
        return file == null ? null : backgrounds.Get(file.Path);
    }

    public virtual Texture GetPanelBackground()
    {
        var croppedBackgrounds = MapSet.Resources?.CroppedBackgroundStore;
        if (croppedBackgrounds == null) return null;

        var file = MapSet.GetFile(Metadata.Background);
        return file == null ? null : croppedBackgrounds.Get(file.Path);
    }

    public virtual Track GetTrack()
    {
        var tracks = MapSet.Resources?.TrackStore;
        Logger.Log($"tracks is null: {tracks == null}");
        if (tracks == null) return null;

        var file = MapSet.GetFile(Metadata.Audio);
        Logger.Log($"Loading track from {file?.Path}");
        return file == null ? null : tracks.Get(file.Path);
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

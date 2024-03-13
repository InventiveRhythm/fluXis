using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Map;
using JetBrains.Annotations;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics.Textures;
using Realms;

namespace fluXis.Game.Database.Maps;

public class RealmMapSet : RealmObject
{
    [PrimaryKey]
    public Guid ID { get; set; }

    public int OnlineID { get; set; } = -1;
    public string Cover { get; set; } = "cover.png";
    public IList<RealmMap> Maps { get; } = null!;

    public DateTimeOffset DateAdded { get; set; } = DateTimeOffset.Now;
    public DateTimeOffset? DateSubmitted { get; set; }
    public DateTimeOffset? DateRanked { get; set; }

    [Ignored]
    public RealmMapMetadata Metadata => Maps.FirstOrDefault()?.Metadata ?? new RealmMapMetadata();

    [Ignored]
    public bool Managed { get; set; }

    [Ignored]
    [CanBeNull]
    public MapResourceProvider Resources { get; set; }

    [Ignored]
    public Action StatusChanged { get; set; } = null!;

    [Ignored]
    [CanBeNull]
    public RealmMap LowestDifficulty => Maps.MinBy(map => map.Filters.NotesPerSecond);

    [Ignored]
    [CanBeNull]
    public RealmMap HighestDifficulty => Maps.MaxBy(map => map.Filters.NotesPerSecond);

    public RealmMapSet([CanBeNull] List<RealmMap> maps = null)
    {
        ID = Guid.NewGuid();
        Maps = maps ?? new List<RealmMap>();
        Maps.ForEach(map => map.MapSet = this);
    }

    [UsedImplicitly]
    public RealmMapSet()
    {
    }

    public virtual Texture GetCover()
    {
        var backgrounds = Resources?.BackgroundStore;
        if (backgrounds == null) return null;

        var texture = backgrounds.Get(GetPathForFile(Cover));
        return texture ?? backgrounds.Get(GetPathForFile(Metadata.Background));
    }

    public virtual string GetPathForFile(string filename) => $"{ID.ToString()}/{filename}";
    public override string ToString() => ID.ToString();

    public void SetStatus(int status)
    {
        foreach (RealmMap map in Maps)
            map.StatusInt = status;

        StatusChanged?.Invoke();
    }
}

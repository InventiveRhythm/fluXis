using System;
using System.Collections.Generic;
using JetBrains.Annotations;
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

    public RealmMap([CanBeNull] RealmMapMetadata meta = null)
    {
        Metadata = meta ?? new RealmMapMetadata();
        ID = Guid.NewGuid();
    }

    [UsedImplicitly]
    private RealmMap()
    {
    }

    public override string ToString()
    {
        return ID.ToString();
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

using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Database.Maps;
using fluXis.Scoring;
using fluXis.Utils;
using JetBrains.Annotations;
using Realms;

namespace fluXis.Database.Score;

public class RealmScore : RealmObject
{
    [PrimaryKey]
    public Guid ID { get; set; }

    public int Version { get; set; } = ScoreManager.SCORE_VERSION;

    public long OnlineID { get; set; } = -1;
    public IList<RealmPlayerScore> Players { get; } = null!;
    public string Mods { get; set; } = string.Empty;
    public Guid MapID { get; set; }
    public DateTimeOffset Date { get; set; }

    public RealmScore(RealmMap map)
    {
        ID = Guid.NewGuid();
        Date = DateTimeOffset.UtcNow;
        MapID = map.ID;
        Players = new List<RealmPlayerScore>();
    }

    [UsedImplicitly]
    private RealmScore()
    {
    }

    public static RealmScore FromScoreInfo(Guid map, ScoreInfo info, long onlineID = -1)
    {
        RealmScore result = new RealmScore
        {
            ID = Guid.NewGuid(),
            OnlineID = onlineID,
            Mods = string.Join(' ', info.Mods),
            MapID = map,
            Date = TimeUtils.GetFromSeconds(info.Timestamp),
        };

        foreach (var playerScore in info.Players)
        {
            result.Players.Add(RealmPlayerScore.FromPlayerScore(playerScore));
        }

        return result;
    }

    //this must be called before closing realm
    public ScoreInfo ToScoreInfo()
    {
        ScoreInfo result = new ScoreInfo
        {
            MapID = -1,
            Timestamp = Date.ToUnixTimeSeconds(),
            Mods = Mods.Split(' ').ToList(),
            Players = new List<PlayerScore>()
        };

        foreach (var realmPlayerScore in Players)
        {
            result.Players.Add(realmPlayerScore.ToPlayerScore());
        }

        return result;
    }
}

using System;
using System.Linq;
using fluXis.Game.Database.Maps;
using fluXis.Game.Utils;
using fluXis.Shared.Scoring;
using fluXis.Shared.Scoring.Enums;
using JetBrains.Annotations;
using Realms;

namespace fluXis.Game.Database.Score;

public class RealmScore : RealmObject
{
    [PrimaryKey]
    public Guid ID { get; set; }

    public int OnlineID { get; set; } = -1;
    public float Accuracy { get; set; }
    public string Grade { get; set; }
    public int Score { get; set; }
    public int MaxCombo { get; set; }
    public int Flawless { get; set; }
    public int Perfect { get; set; }
    public int Great { get; set; }
    public int Alright { get; set; }
    public int Okay { get; set; }
    public int Miss { get; set; }
    public string Mods { get; set; } = string.Empty;
    public Guid MapID { get; set; }
    public DateTimeOffset Date { get; set; }
    public long PlayerID { get; set; }

    [Ignored]
    public ScoreRank Rank
    {
        get => Enum.Parse<ScoreRank>(Grade);
        set => Grade = value.ToString();
    }

    public RealmScore(RealmMap map)
    {
        ID = Guid.NewGuid();
        Date = DateTimeOffset.UtcNow;
        MapID = map.ID;
    }

    [UsedImplicitly]
    private RealmScore()
    {
    }

    public static RealmScore FromScoreInfo(RealmMap map, ScoreInfo info, int onlineID = -1)
    {
        return new RealmScore
        {
            ID = Guid.NewGuid(),
            OnlineID = onlineID,
            Accuracy = info.Accuracy,
            Rank = info.Rank,
            Score = info.Score,
            MaxCombo = info.MaxCombo,
            Flawless = info.Flawless,
            Perfect = info.Perfect,
            Great = info.Great,
            Alright = info.Alright,
            Okay = info.Okay,
            Miss = info.Miss,
            Mods = string.Join(' ', info.Mods),
            MapID = map.ID,
            Date = TimeUtils.GetFromSeconds(info.Timestamp),
            PlayerID = info.PlayerID
        };
    }

    public ScoreInfo ToScoreInfo()
    {
        return new ScoreInfo
        {
            Accuracy = Accuracy,
            Rank = Rank,
            Score = Score,
            Combo = 0,
            MaxCombo = MaxCombo,
            Flawless = Flawless,
            Perfect = Perfect,
            Great = Great,
            Alright = Alright,
            Okay = Okay,
            Miss = Miss,
            HitResults = null,
            PlayerID = PlayerID,
            MapID = -1,
            MapHash = null,
            Timestamp = Date.ToUnixTimeSeconds(),
            Mods = Mods.Split(' ').ToList()
        };
    }
}

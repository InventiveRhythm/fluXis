using System;
using fluXis.Game.Database.Maps;
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

    public RealmJudgements Judgements { get; set; }

    public string Mods { get; set; } = string.Empty;

    public Guid MapID { get; set; }

    public DateTimeOffset Date { get; set; }

    public RealmScore(RealmMap map)
    {
        ID = Guid.NewGuid();
        Date = DateTimeOffset.Now;
        MapID = map.ID;
    }

    [UsedImplicitly]
    private RealmScore()
    {
    }
}

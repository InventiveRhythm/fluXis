using System;
using fluXis.Game.Database.Maps;
using fluXis.Game.Online;
using fluXis.Game.Online.API.Users;
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

    public int PlayerID { get; set; }

    [Ignored]
    public APIUser Player => UserCache.GetUser(PlayerID);

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

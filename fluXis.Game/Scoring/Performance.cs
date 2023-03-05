using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Map;
using Newtonsoft.Json;

namespace fluXis.Game.Scoring;

public class Performance
{
    [JsonProperty("accuracy")]
    public float Accuracy { get; private set; }

    [JsonProperty("grade")]
    public Grade Grade { get; private set; }

    [JsonProperty("score")]
    public int Score { get; private set; }

    [JsonProperty("combo")]
    public int Combo { get; private set; }

    [JsonProperty("maxCombo")]
    public int MaxCombo { get; private set; }

    [JsonProperty("judgements")]
    public Dictionary<Judgements, int> Judgements { get; }

    [JsonProperty("hitPoints")]
    public List<HitPoint> HitPoints { get; }

    [JsonProperty("mapid")]
    public string MapID { get; private set; }

    [JsonProperty("maphash")]
    public string MapHash { get; private set; }

    [JsonIgnore]
    public readonly MapInfo Map;

    public Performance(MapInfo map)
    {
        Map = map;
        MapID = map.ID;
        MapHash = map.MD5;

        Accuracy = 0;
        Grade = Grade.X;
        Score = 0;
        Combo = 0;
        MaxCombo = 0;
        Judgements = new Dictionary<Judgements, int>();
        HitPoints = new List<HitPoint>();
    }

    public void AddHitPoint(HitPoint hitPoint)
    {
        HitPoints.Add(hitPoint);
    }

    public void AddJudgement(Judgements jud)
    {
        if (Judgements.ContainsKey(jud))
            Judgements[jud]++;
        else
            Judgements.Add(jud, 1);

        Calculate();
    }

    public void IncCombo()
    {
        Combo++;
        if (Combo > MaxCombo)
            MaxCombo = Combo;
    }

    public void ResetCombo()
    {
        Combo = 0;
    }

    public void Calculate()
    {
        if (GetAllJudgements() == 0)
            Accuracy = 100;
        else
            Accuracy = GetRated() / GetAllJudgements() * 100;

        Score = (int)(GetRated() / Map.MaxCombo * 1000000);
        calculateGrade();
    }

    private void calculateGrade()
    {
        Grade = Accuracy switch
        {
            100 => Grade.X,
            >= 99 => Grade.SS,
            >= 98 => Grade.S,
            >= 95 => Grade.AA,
            >= 90 => Grade.A,
            >= 80 => Grade.B,
            >= 70 => Grade.C,
            _ => Grade.D
        };
    }

    public int GetHit()
    {
        return (from j in Judgements let j2 = HitWindow.FromKey(j.Key) where j2 is { Accuracy: > 0 } select j.Value).Sum();
    }

    public float GetRated()
    {
        return (from j in Judgements let j2 = HitWindow.FromKey(j.Key) select j.Value * j2.Accuracy).Sum();
    }

    public int GetMiss()
    {
        return (from j in Judgements where j.Key == Scoring.Judgements.Miss select j.Value).Sum();
    }

    public int GetAllJudgements()
    {
        return GetHit() + GetMiss();
    }

    public int GetJudgementCount(Judgements jud)
    {
        if (Judgements.ContainsKey(jud))
            return Judgements[jud];

        return 0;
    }

    public bool IsFullCombo()
    {
        return GetMiss() == 0;
    }

    public bool IsAllFlawless()
    {
        foreach (var (key, count) in Judgements)
        {
            if (key != Scoring.Judgements.Flawless && count > 0)
                return false;
        }

        return true;
    }
}

public class HitPoint
{
    [JsonProperty("time")]
    public float Time { get; }

    [JsonProperty("difference")]
    public float Difference { get; }

    [JsonProperty("judgement")]
    public Judgements Judgement { get; }

    public HitPoint(float time, float diff, Judgements jud)
    {
        Time = time;
        Difference = diff;
        Judgement = jud;
    }
}

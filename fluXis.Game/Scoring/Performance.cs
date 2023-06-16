using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Map;
using fluXis.Game.Mods;
using Newtonsoft.Json;

namespace fluXis.Game.Scoring;

public class Performance
{
    [JsonProperty("accuracy")]
    public float Accuracy
    {
        get
        {
            if (NotesTotal == 0) return 100;

            return NotesRated / NotesTotal * 100;
        }
    }

    [JsonProperty("grade")]
    public Grade Grade => Accuracy switch
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

    [JsonProperty("score")]
    public int Score
    {
        get
        {
            var maxScore = (int)(1000000 * ScoreMultiplier);
            var accBased = (int)(NotesRated / Map.MaxCombo * (maxScore * .9f));
            var comboBased = (int)(MaxCombo / (float)Map.MaxCombo * (maxScore * .1f));
            return accBased + comboBased;
        }
    }

    [JsonProperty("combo")]
    public int Combo { get; set; }

    [JsonProperty("maxCombo")]
    public int MaxCombo { get; set; }

    [JsonProperty("judgements")]
    public Dictionary<Judgement, int> Judgements { get; }

    [JsonProperty("hitPoints")]
    public List<HitStat> HitStats { get; }

    [JsonProperty("mapid")]
    public int MapID { get; private set; }

    [JsonProperty("maphash")]
    public string MapHash { get; private set; }

    [JsonProperty("mods")]
    public List<string> ModNames => Mods.Select(m => m.Acronym).ToList();

    [JsonIgnore]
    public readonly MapInfo Map;

    [JsonIgnore]
    public int TotalNotesHit => (from j in Judgements let j2 = HitWindow.FromKey(j.Key) where j2 is { Accuracy: > 0 } select j.Value).Sum();

    [JsonIgnore]
    public float NotesRated => (from j in Judgements let j2 = HitWindow.FromKey(j.Key) select j.Value * j2.Accuracy).Sum();

    [JsonIgnore]
    public int NotesMissed => (from j in Judgements where j.Key == Judgement.Miss select j.Value).Sum();

    [JsonIgnore]
    public int NotesTotal => TotalNotesHit + NotesMissed;

    [JsonIgnore]
    public bool FullCombo => NotesMissed == 0;

    [JsonIgnore]
    public bool AllFlawless => Judgements.All(j => j.Key == Judgement.Flawless);

    [JsonIgnore]
    public Action<HitStat> OnHitStatAdded;

    [JsonIgnore]
    public List<IMod> Mods { get; }

    [JsonIgnore]
    public bool IsRanked => Mods.All(m => m.Rankable);

    [JsonIgnore]
    public float ScoreMultiplier => 1f + Mods.Sum(mod => mod.ScoreMultiplier - 1f);

    public Performance(MapInfo map, int mapid, string hash, List<IMod> mods = null)
    {
        Map = map;
        MapID = mapid;
        MapHash = hash;
        Mods = mods ?? new List<IMod>();

        Combo = 0;
        MaxCombo = 0;
        Judgements = new Dictionary<Judgement, int>();
        HitStats = new List<HitStat>();
    }

    public void AddHitStat(HitStat hitStat)
    {
        HitStats.Add(hitStat);
        OnHitStatAdded?.Invoke(hitStat);
    }

    public void AddJudgement(Judgement jud)
    {
        if (Judgements.ContainsKey(jud))
            Judgements[jud]++;
        else
            Judgements.Add(jud, 1);
    }

    public void IncCombo()
    {
        Combo++;
        MaxCombo = Math.Max(Combo, MaxCombo);
    }

    public void ResetCombo()
    {
        Combo = 0;
    }

    public int GetJudgementCount(Judgement jud) => Judgements.ContainsKey(jud) ? Judgements[jud] : 0;
}

public class HitStat
{
    [JsonProperty("time")]
    public float Time { get; }

    [JsonProperty("difference")]
    public float Difference { get; }

    [JsonProperty("judgement")]
    public Judgement Judgement { get; }

    public HitStat(float time, float diff, Judgement jud)
    {
        Time = time;
        Difference = diff;
        Judgement = jud;
    }
}

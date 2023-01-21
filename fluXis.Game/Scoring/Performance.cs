using System;
using System.Collections.Generic;
using fluXis.Game.Map;
using Newtonsoft.Json;

namespace fluXis.Game.Scoring
{
    public class Performance
    {
        [JsonProperty("accuracy")]
        public float Accuracy { get; private set; }

        [JsonProperty("grade")]
        public string Grade { get; private set; }

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
            Grade = "X";
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

            Accuracy = (float)Math.Round(Accuracy, 2);

            Score = (int)(GetRated() / Map.MaxCombo * 1000000);
            calculateGrade();
        }

        private void calculateGrade()
        {
            if (Accuracy == 100)
                Grade = "X";
            else if (Accuracy >= 99)
                Grade = "SS";
            else if (Accuracy >= 98)
                Grade = "S";
            else if (Accuracy >= 95)
                Grade = "AA";
            else if (Accuracy >= 90)
                Grade = "A";
            else if (Accuracy >= 80)
                Grade = "B";
            else if (Accuracy >= 70)
                Grade = "C";
            else
                Grade = "D";
        }

        public int GetHit()
        {
            int total = 0;

            foreach (var j in Judgements)
            {
                HitWindow j2 = HitWindow.FromKey(j.Key);
                if (j2 != null && j2.Accuracy > 0)
                    total += j.Value;
            }

            return total;
        }

        public float GetRated()
        {
            float val = 0;

            foreach (var j in Judgements)
            {
                HitWindow j2 = HitWindow.FromKey(j.Key);
                val += j.Value * j2.Accuracy;
            }

            return val;
        }

        public int GetMiss()
        {
            int total = 0;

            if (Judgements.ContainsKey(Scoring.Judgements.Miss))
                total += Judgements[Scoring.Judgements.Miss];

            return total;
        }

        public int GetAllJudgements()
        {
            return GetHit() + GetMiss();
        }

        public int GetJudgementCount(Judgements jud)
        {
            if (Judgements.ContainsKey(jud))
                return Judgements[jud];
            else
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
}

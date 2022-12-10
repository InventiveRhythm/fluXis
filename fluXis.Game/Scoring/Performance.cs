using System;
using System.Collections.Generic;
using fluXis.Game.Map;

namespace fluXis.Game.Scoring
{
    public class Performance
    {
        public float Accuracy { get; private set; }
        public string Grade { get; private set; }
        public int Score { get; private set; }
        public int Combo { get; private set; }
        public int MaxCombo { get; private set; }
        public Dictionary<Judgements, int> Judgements { get; }
        private MapInfo mapInfo { get; set; }

        public Performance()
        {
            Accuracy = 0;
            Grade = "X";
            Score = 0;
            Combo = 0;
            MaxCombo = 0;
            Judgements = new Dictionary<Judgements, int>();
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

            if (mapInfo != null)
            {
                int totalHitable = 0;

                foreach (var h in mapInfo.HitObjects)
                {
                    totalHitable++;
                    if (h.IsLongNote())
                        totalHitable++;
                }

                Score = (int)(GetRated() / totalHitable * 100000);
            }
        }

        public int GetHit()
        {
            int total = 0;

            foreach (var j in Judgements)
            {
                Judgement j2 = Judgement.FromKey(j.Key);
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
                Judgement j2 = Judgement.FromKey(j.Key);
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

        public void SetMapInfo(MapInfo mapInfo)
        {
            this.mapInfo = mapInfo;
        }
    }
}

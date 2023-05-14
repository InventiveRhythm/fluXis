using System.Collections.Generic;
using fluXis.Game.Database.Maps;
using fluXis.Game.Database.Score;
using fluXis.Game.Scoring;
using fluXis.Game.Screens.Select.Info.Scores;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Tests.Select;

public partial class TestScoreEntry : FluXisTestScene
{
    public TestScoreEntry()
    {
        var score = new RealmScore(RealmMap.CreateNew())
        {
            Score = 1000000,
            Accuracy = 100,
            MaxCombo = 1000,
            Grade = "X",
            Judgements = new RealmJudgements(new Dictionary<Judgement, int>
            {
                { Judgement.Flawless, 100 },
                { Judgement.Perfect, 100 },
                { Judgement.Great, 100 },
                { Judgement.Alright, 100 },
                { Judgement.Okay, 100 },
                { Judgement.Miss, 100 }
            })
        };

        score.Mods.Add("HD");
        score.Mods.Add("NF");

        Add(new Container
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Width = 0.5f,
            Children = new Drawable[]
            {
                new ScoreListEntry(score, 1)
            }
        });
    }
}

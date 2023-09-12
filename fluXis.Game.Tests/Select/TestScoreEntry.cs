using fluXis.Game.Database.Maps;
using fluXis.Game.Database.Score;
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
            Mods = "HD NF",
            Flawless = 100,
            Perfect = 100,
            Great = 100,
            Alright = 100,
            Okay = 100,
            Miss = 100
        };

        Add(new Container
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Width = 0.5f,
            Children = new Drawable[]
            {
                new ScoreListEntry()
            }
        });
    }
}

using System;
using System.Linq;
using fluXis.Graphics.Drawables;
using fluXis.Scoring.Enums;
using NUnit.Framework;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Tests.Graphics;

public partial class TestDrawableScoreRanks : FluXisTestScene
{
    [Test]
    public void TestRanks()
    {
        AddStep("add ranks", () =>
        {
            Child = new FillFlowContainer
            {
                Direction = FillDirection.Horizontal,
                ChildrenEnumerable = Enum.GetValues<ScoreRank>().Select(x => new DrawableScoreRank
                {
                    Rank = x,
                    FontSize = 64,
                    Size = new Vector2(96)
                })
            };
        });
    }
}

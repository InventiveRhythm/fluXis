using fluXis.Graphics.Drawables;
using fluXis.Scoring.Enums;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Tests.Graphics;

public partial class TestRanks : FluXisTestScene
{
    public TestRanks()
    {
        Add(new FillFlowContainer
        {
            Direction = FillDirection.Horizontal,
            Children = new Drawable[]
            {
                new DrawableScoreRank { Rank = ScoreRank.X, FontSize = 64 },
                new DrawableScoreRank { Rank = ScoreRank.SS, FontSize = 64 },
                new DrawableScoreRank { Rank = ScoreRank.S, FontSize = 64 },
                new DrawableScoreRank { Rank = ScoreRank.AA, FontSize = 64 },
                new DrawableScoreRank { Rank = ScoreRank.A, FontSize = 64 },
                new DrawableScoreRank { Rank = ScoreRank.B, FontSize = 64 },
                new DrawableScoreRank { Rank = ScoreRank.C, FontSize = 64 },
                new DrawableScoreRank { Rank = ScoreRank.D, FontSize = 64 }
            }
        });
    }
}

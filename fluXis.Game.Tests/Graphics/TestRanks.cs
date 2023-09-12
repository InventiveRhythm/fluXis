using fluXis.Game.Graphics.Drawables;
using fluXis.Game.Scoring.Enums;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Tests.Graphics;

public partial class TestRanks : FluXisTestScene
{
    public TestRanks()
    {
        Add(new FillFlowContainer
        {
            Direction = FillDirection.Horizontal,
            Children = new Drawable[]
            {
                new DrawableScoreRank { Rank = ScoreRank.X, Size = 64 },
                new DrawableScoreRank { Rank = ScoreRank.SS, Size = 64 },
                new DrawableScoreRank { Rank = ScoreRank.S, Size = 64 },
                new DrawableScoreRank { Rank = ScoreRank.AA, Size = 64 },
                new DrawableScoreRank { Rank = ScoreRank.A, Size = 64 },
                new DrawableScoreRank { Rank = ScoreRank.B, Size = 64 },
                new DrawableScoreRank { Rank = ScoreRank.C, Size = 64 },
                new DrawableScoreRank { Rank = ScoreRank.D, Size = 64 }
            }
        });
    }
}

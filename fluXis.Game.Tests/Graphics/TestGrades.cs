using fluXis.Game.Graphics;
using fluXis.Game.Scoring;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Tests.Graphics;

public partial class TestGrades : FluXisTestScene
{
    public TestGrades()
    {
        Add(new FillFlowContainer
        {
            Direction = FillDirection.Horizontal,
            Children = new Drawable[]
            {
                new DrawableGrade { Grade = Grade.X, Size = 64 },
                new DrawableGrade { Grade = Grade.SS, Size = 64 },
                new DrawableGrade { Grade = Grade.S, Size = 64 },
                new DrawableGrade { Grade = Grade.AA, Size = 64 },
                new DrawableGrade { Grade = Grade.A, Size = 64 },
                new DrawableGrade { Grade = Grade.B, Size = 64 },
                new DrawableGrade { Grade = Grade.C, Size = 64 },
                new DrawableGrade { Grade = Grade.D, Size = 64 }
            }
        });
    }
}

using fluXis.Game.Graphics;
using fluXis.Game.Scoring;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Game.Screens.Result.UI;

public partial class ResultJudgement : FillFlowContainer
{
    public ResultJudgement(HitWindow hitWindow, int count)
    {
        Direction = FillDirection.Horizontal;
        AutoSizeAxes = Axes.Both;
        Spacing = new Vector2(5, 0);

        AddRange(new Drawable[]
        {
            new SpriteText
            {
                Text = hitWindow.Key.ToString(),
                Font = FluXisFont.Default(24),
                Colour = hitWindow.Color
            },
            new SpriteText
            {
                Text = count.ToString(),
                Font = FluXisFont.Default(24)
            }
        });
    }
}

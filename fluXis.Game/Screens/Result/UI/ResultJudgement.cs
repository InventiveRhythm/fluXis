using fluXis.Game.Graphics;
using fluXis.Game.Scoring;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
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
            new FluXisSpriteText
            {
                Text = hitWindow.Key.ToString(),
                FontSize = 24,
                Colour = hitWindow.Color
            },
            new FluXisSpriteText
            {
                Text = count.ToString(),
                FontSize = 24
            }
        });
    }
}

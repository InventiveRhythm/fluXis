using fluXis.Game.Graphics;
using fluXis.Game.Scoring;
using fluXis.Game.Skinning;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Game.Screens.Result.UI;

public partial class ResultJudgement : FillFlowContainer
{
    public HitWindow HitWindow { get; init; }
    public new int Count { get; init; }

    [BackgroundDependencyLoader]
    private void load(SkinManager skinManager)
    {
        Direction = FillDirection.Horizontal;
        AutoSizeAxes = Axes.Both;
        Spacing = new Vector2(5, 0);

        AddRange(new Drawable[]
        {
            new FluXisSpriteText
            {
                Text = HitWindow.Key.ToString(),
                FontSize = 24,
                Colour = skinManager.CurrentSkin.GetColorForJudgement(HitWindow.Key)
            },
            new FluXisSpriteText
            {
                Text = Count.ToString(),
                FontSize = 24
            }
        });
    }
}

using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Scoring.Enums;
using fluXis.Game.Skinning;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Game.Screens.Result.UI;

public partial class ResultJudgement : FillFlowContainer
{
    public Judgement Judgement { get; init; }
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
                Text = Judgement.ToString(),
                FontSize = 24,
                Colour = skinManager.SkinJson.GetColorForJudgement(Judgement)
            },
            new FluXisSpriteText
            {
                Text = Count.ToString(),
                FontSize = 24
            }
        });
    }
}

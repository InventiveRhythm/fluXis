using fluXis.Game.Skinning.Json;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Skinning.Default.Lighting;

public partial class DefaultColumnLighing : VisibilityContainer
{
    private readonly SkinJson skinJson;

    public DefaultColumnLighing(SkinJson skinJson)
    {
        this.skinJson = skinJson;

        Anchor = Anchor.BottomCentre;
        Origin = Anchor.BottomCentre;
        RelativeSizeAxes = Axes.X;
        Height = 400;
        Alpha = 0;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Alpha = .5f
            }
        };
    }

    public void UpdateColor(int lane, int maxLanes)
    {
        var color = skinJson.GetLaneColor(lane, maxLanes);
        Colour = ColourInfo.GradientVertical(color.Opacity(0), color);
    }

    protected override void PopIn() => this.FadeIn();
    protected override void PopOut() => this.FadeOut(300, Easing.Out);
}

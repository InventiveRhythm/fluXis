using fluXis.Skinning.Json;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Skinning.Default.Lighting;

public partial class DefaultColumnLighting : VisibilityContainer
{
    private readonly SkinJson skinJson;

    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private ICustomColorProvider colorProvider { get; set; }

    public DefaultColumnLighting(SkinJson skinJson)
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
        Schedule(() =>
        {
            if (colorProvider == null || !colorProvider.HasColorFor(lane, maxLanes, out var color))
                color = skinJson.GetLaneColor(lane, maxLanes);

            Colour = ColourInfo.GradientVertical(color.Opacity(0), color);
        });
    }

    protected override void PopIn() => this.FadeIn();
    protected override void PopOut() => this.FadeOut(300, Easing.Out);
}

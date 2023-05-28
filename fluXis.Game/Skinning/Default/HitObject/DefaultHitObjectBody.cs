using fluXis.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Skinning.Default.HitObject;

public partial class DefaultHitObjectBody : Container
{
    private Box box;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Width = 0.9f;
        Anchor = Anchor.BottomCentre;
        Origin = Anchor.BottomCentre;
        Child = box = new Box
        {
            RelativeSizeAxes = Axes.Both
        };
    }

    public void UpdateColor(int lane, int keyCount)
    {
        var color = FluXisColors.GetLaneColor(lane, keyCount);
        box.Colour = ColourInfo.GradientVertical(color.Darken(.4f), color);
    }
}

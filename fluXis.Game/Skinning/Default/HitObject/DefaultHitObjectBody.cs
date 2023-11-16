using fluXis.Game.Skinning.Json;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK.Graphics;

namespace fluXis.Game.Skinning.Default.HitObject;

public partial class DefaultHitObjectBody : Container
{
    private readonly SkinJson skinJson;
    private readonly Box box;

    public DefaultHitObjectBody(SkinJson skinJson)
    {
        this.skinJson = skinJson;

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
        var color = skinJson.GetLaneColor(lane, keyCount);
        SetColor(color, color.Darken(.4f));
    }

    public void SetColor(Color4 start, Color4 end) => box.Colour = ColourInfo.GradientVertical(end, start);
}

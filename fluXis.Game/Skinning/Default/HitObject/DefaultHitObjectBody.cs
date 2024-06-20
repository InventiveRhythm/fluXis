using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Skinning.Bases.HitObjects;
using fluXis.Game.Skinning.Json;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Skinning.Default.HitObject;

public partial class DefaultHitObjectBody : DefaultSkinDrawable, ICanHaveSnapColor
{
    private readonly Box box;

    public DefaultHitObjectBody(SkinJson skinJson)
        : base(skinJson)
    {
        RelativeSizeAxes = Axes.X;
        Width = 0.9f;
        Anchor = Anchor.BottomCentre;
        Origin = Anchor.BottomCentre;
        InternalChild = box = new Box
        {
            RelativeSizeAxes = Axes.Both
        };
    }

    protected override void SetColor(Colour4 color)
        => box.Colour = ColourInfo.GradientVertical(color.Darken(.4f), color);

    public void ApplySnapColor(int start, int end)
    {
        UseCustomColor = true;
        var startColor = FluXisColors.GetSnapColor(start);
        var endColor = FluXisColors.GetSnapColor(end).Darken(.4f);
        box.Colour = ColourInfo.GradientVertical(endColor, startColor);
    }
}

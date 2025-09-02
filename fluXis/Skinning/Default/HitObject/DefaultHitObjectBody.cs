using fluXis.Graphics.UserInterface.Color;
using fluXis.Skinning.Bases;
using fluXis.Skinning.Bases.HitObjects;
using fluXis.Skinning.Json;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Skinning.Default.HitObject;

public partial class DefaultHitObjectBody : ColorableSkinDrawable, ICanHaveSnapColor
{
    public DefaultHitObjectBody(SkinJson skinJson, MapColor index)
        : base(skinJson, index)
    {
        RelativeSizeAxes = Axes.X;
        Width = 0.9f;
        Anchor = Anchor.BottomCentre;
        Origin = Anchor.BottomCentre;
        InternalChild = new Box
        {
            RelativeSizeAxes = Axes.Both,
            Colour = ColourInfo.GradientVertical(Colour4.White.Darken(.4f), Colour4.White)
        };
    }

    public override void SetColor(Colour4 color)
        => Colour = color;

    public override void FadeColor(Colour4 color, double duration = 0, Easing easing = Easing.None)
        => this.FadeColour(color, duration, easing);

    public void ApplySnapColor(int start, int end)
    {
        UseCustomColor = true;
        var startColor = SkinJson.SnapColors.GetColor(start);
        var endColor = SkinJson.SnapColors.GetColor(end);
        Colour = ColourInfo.GradientVertical(endColor, startColor);
    }
}

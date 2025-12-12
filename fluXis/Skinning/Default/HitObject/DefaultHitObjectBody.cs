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
    public Box BoxSprite { get; }

    public DefaultHitObjectBody(SkinJson skinJson, MapColor index)
        : base(skinJson, index)
    {
        InternalChild = BoxSprite = new Box
        {
            RelativeSizeAxes = Axes.Both,
            Width = 0.9f,
            Anchor = Anchor.BottomCentre,
            Origin = Anchor.BottomCentre,
            Colour = ColourInfo.GradientVertical(Colour4.White.Darken(.4f), Colour4.White)
        };
    }

    public override void SetColor(Colour4 color)
        => Colour = color;

    public void ApplySnapColor(int start, int end)
    {
        UseCustomColor = true;
        var startColor = SkinJson.SnapColors.GetColor(start);
        var endColor = SkinJson.SnapColors.GetColor(end);
        Colour = ColourInfo.GradientVertical(endColor, startColor);
    }
}

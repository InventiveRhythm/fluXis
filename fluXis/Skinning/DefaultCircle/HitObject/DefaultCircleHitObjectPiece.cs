using fluXis.Graphics.UserInterface.Color;
using fluXis.Skinning.Bases;
using fluXis.Skinning.Bases.HitObjects;
using fluXis.Skinning.Json;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Skinning.DefaultCircle.HitObject;

public partial class DefaultCircleHitObjectPiece : ColorableSkinDrawable, ICanHaveSnapColor
{
    protected Circle Circle { get; }

    public DefaultCircleHitObjectPiece(SkinJson skinJson, MapColor index)
        : base(skinJson, index)
    {
        InternalChild = Circle = new Circle
        {
            RelativeSizeAxes = Axes.Both,
            Size = new Vector2(DefaultCircleSkin.SCALE),
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            BorderColour = ColourInfo.GradientVertical(Colour4.White.Lighten(0.2f), Colour4.White.Darken(.4f)),
            BorderThickness = 8,
        };
    }

    protected override void Update()
    {
        base.Update();
        Height = DrawWidth;
    }

    public override void SetColor(Colour4 color) => Circle.Colour = color;

    public void ApplySnapColor(int start, int end)
    {
        UseCustomColor = true;
        SetColor(SkinJson.SnapColors.GetColor(start));
    }
}

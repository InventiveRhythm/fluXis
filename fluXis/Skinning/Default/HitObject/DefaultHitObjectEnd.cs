using fluXis.Skinning.Bases;
using fluXis.Skinning.Bases.HitObjects;
using fluXis.Skinning.Json;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Skinning.Default.HitObject;

public partial class DefaultHitObjectEnd : ColorableSkinDrawable, ICanHaveSnapColor
{
    private readonly Box box;

    public DefaultHitObjectEnd(SkinJson skinJson)
        : base(skinJson)
    {
        CornerRadius = 10;
        Masking = true;
        Height = 42;
        RelativeSizeAxes = Axes.X;
        Anchor = Anchor.BottomCentre;
        Origin = Anchor.BottomCentre;
        InternalChild = box = new Box
        {
            RelativeSizeAxes = Axes.Both
        };
    }

    protected override void Update()
    {
        var factor = DrawWidth / 114f;
        Height = 42f * factor;
    }

    protected override void SetColor(Colour4 color) => box.Colour = color.Darken(.4f);

    public void ApplySnapColor(int start, int end)
    {
        UseCustomColor = true;
        SetColor(SkinJson.SnapColors.GetColor(start));
    }
}

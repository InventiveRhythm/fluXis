using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Skinning.Bases;
using fluXis.Game.Skinning.Bases.HitObjects;
using fluXis.Game.Skinning.Json;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Skinning.Default.HitObject;

public partial class DefaultHitObjectPiece : ColorableSkinDrawable, ICanHaveSnapColor
{
    private readonly Box box;

    public DefaultHitObjectPiece(SkinJson skinJson)
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

    protected override void SetColor(Colour4 color) => box.Colour = color;

    public void ApplySnapColor(int start, int end)
    {
        UseCustomColor = true;
        SetColor(FluXisColors.GetSnapColor(start));
    }
}

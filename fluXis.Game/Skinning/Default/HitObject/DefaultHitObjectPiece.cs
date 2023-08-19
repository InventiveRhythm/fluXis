using fluXis.Game.Graphics.UserInterface.Color;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Skinning.Default.HitObject;

public partial class DefaultHitObjectPiece : Container
{
    private readonly Box box;

    public DefaultHitObjectPiece()
    {
        CornerRadius = 10;
        Masking = true;
        Height = 42;
        RelativeSizeAxes = Axes.X;
        Anchor = Anchor.BottomCentre;
        Origin = Anchor.BottomCentre;
        Child = box = new Box
        {
            RelativeSizeAxes = Axes.Both
        };
    }

    protected override void Update()
    {
        var factor = DrawWidth / 114f;
        Height = 42f * factor;
    }

    public void UpdateColor(int lane, int keyCount) => SetColor(FluXisColors.GetLaneColor(lane, keyCount));
    public void SetColor(Colour4 color) => box.Colour = color;
}

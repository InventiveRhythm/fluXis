using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.UserInterface;
using osuTK;

namespace fluXis.Skinning.DefaultCircle.HitObject;

public partial class DefaultCircleLandmine : CompositeDrawable
{
    private CircularProgress progress;

    public DefaultCircleLandmine(bool small)
    {
        RelativeSizeAxes = Axes.X;
        Anchor = Anchor.BottomCentre;
        Origin = Anchor.BottomCentre;
        Colour = Colour4.FromHex("#FF5252");

        InternalChild = progress = new CircularProgress
        {
            RelativeSizeAxes = Axes.Both,
            Size = new Vector2(DefaultCircleSkin.SCALE),
            RoundedCaps = true,
            Progress = 1,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
        };
    }

    protected override void Update()
    {
        base.Update();
        Height = DrawWidth;

        var factor = 8 / (progress.DrawWidth / 4);
        progress.InnerRadius = factor;
    }
}

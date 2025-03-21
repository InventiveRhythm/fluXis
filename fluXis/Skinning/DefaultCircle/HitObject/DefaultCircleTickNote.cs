using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.UserInterface;
using osuTK;

namespace fluXis.Skinning.DefaultCircle.HitObject;

public partial class DefaultCircleTickNote : CompositeDrawable
{
    private CircularProgress progress;

    public DefaultCircleTickNote(bool small)
    {
        RelativeSizeAxes = Axes.X;
        Anchor = Anchor.BottomCentre;
        Origin = Anchor.BottomCentre;
        Colour = Colour4.FromHex("#F2C979");

        InternalChild = progress = new CircularProgress
        {
            RelativeSizeAxes = Axes.Both,
            Size = new Vector2(DefaultCircleSkin.SCALE),
            RoundedCaps = true,
            Progress = small ? .35f : .5f,
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
        progress.Rotation = (float)(360 * (.25f + (.5f - progress.Progress) / 2));
    }
}

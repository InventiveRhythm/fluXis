using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Graphics.Containers;

public partial class AspectRatioContainer : Container
{
    private readonly Bindable<bool> enabled;

    public AspectRatioContainer(Bindable<bool> enabled)
    {
        this.enabled = enabled;

        RelativeSizeAxes = Axes.None;
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
    }

    protected override void Update()
    {
        base.Update();

        if (Parent is null)
            return;

        var p = Parent.Padding;
        var vPad = p.Top + p.Bottom;
        var hPad = p.Left + p.Right;
        var pW = Parent.DrawWidth - hPad;
        var pH = Parent.DrawHeight - vPad;

        Size = new Vector2(pW, pH);

        if (enabled.Value)
        {
            const float target_aspect = 1920 / 1080f;

            var currentAspect = pW / pH;

            if (currentAspect < target_aspect)
            {
                Width = pW;
                Height = DrawWidth / target_aspect;
            }
            else
            {
                Width = DrawHeight * target_aspect;
                Height = pH;
            }
        }
    }
}

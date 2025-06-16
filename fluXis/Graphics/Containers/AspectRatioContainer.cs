using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Graphics.Containers;

public partial class AspectRatioContainer : Container
{
    public Vector2 TargetSize { get; set; } = new(1920, 1080);

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

        if (!enabled.Value) return;

        var targetAspect = TargetSize.X / TargetSize.Y;
        var currentAspect = pW / pH;

        if (currentAspect < targetAspect)
        {
            Width = pW;
            Height = DrawWidth / targetAspect;
        }
        else
        {
            Width = DrawHeight * targetAspect;
            Height = pH;
        }
    }
}

using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Layout;
using osuTK;

namespace fluXis.Graphics.Containers;

#nullable enable

public partial class Marquee : CompositeDrawable
{
    public new Axes AutoSizeAxes
    {
        get => base.AutoSizeAxes;
        set => base.AutoSizeAxes = value;
    }

    public Anchor StaticAnchor { get; set; } = Anchor.TopLeft;

    public Func<Drawable>? CreateFunc
    {
        get => createFunc;
        set
        {
            createFunc = value;
            if (IsLoaded) refreshContent();
        }
    }

    private Func<Drawable>? createFunc;
    private readonly LayoutValue sizeLayout;
    private bool needsAnimationUpdate;

    private FillFlowContainer flow = null!;
    private Drawable? main;
    private Drawable? extra;

    public Marquee()
    {
        AutoSizeAxes = Axes.Y;
        Masking = true;
        AddLayout(sizeLayout = new LayoutValue(Invalidation.DrawSize));
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChild = flow = new FillFlowContainer
        {
            AutoSizeAxes = Axes.Both,
            Direction = FillDirection.Horizontal,
            Spacing = new Vector2(64)
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        refreshContent();
    }

    private void refreshContent()
    {
        flow.Clear();
        if (createFunc is null) return;

        flow.Add(main = createFunc());
        flow.Add(extra = createFunc().With(x => x.Alpha = 0));
        needsAnimationUpdate = true;
    }

    protected override void UpdateAfterChildren()
    {
        base.UpdateAfterChildren();

        if (!needsAnimationUpdate && sizeLayout.IsValid)
            return;

        if (main is null || extra is null)
            return;

        var overflow = main.DrawWidth - DrawWidth;

        if (overflow > 0)
        {
            extra.Alpha = 1;
            flow.Anchor = flow.Origin = Anchor.TopLeft;

            var target = main.DrawWidth + flow.Spacing.X;
            flow.MoveToX(0).Delay(800).MoveToX(-target, target / 64 * 1000, Easing.InOutQuad).Loop();
        }
        else
        {
            extra.Alpha = 0;
            flow.Anchor = flow.Origin = StaticAnchor;

            flow.ClearTransforms();
            flow.MoveToX(0);
        }

        sizeLayout.Validate();
        needsAnimationUpdate = false;
    }
}

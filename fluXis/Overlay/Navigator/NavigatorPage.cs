using System;
using fluXis.Online.Drawables;
using fluXis.Online.Fluxel;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Overlay.Navigator;

#nullable enable

[LongRunningLoad]
public abstract partial class NavigatorPage<T> : NavigatorPage
    where T : class
{
    [Resolved]
    protected IAPIClient API { get; private set; } = null!;

    [Resolved]
    protected FluXisGame? Game { get; private set; }

    protected virtual float ContentWidth => 0f;
    protected abstract T PullData();
    protected abstract Drawable CreateContent(T data);

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Y;
        Anchor = Origin = Anchor.TopCentre;

        if (ContentWidth > 0)
            Width = ContentWidth;
        else
            RelativeSizeAxes = Axes.X;

        try
        {
            var data = PullData();
            InternalChild = CreateContent(data);
        }
        catch (Exception ex)
        {
            InternalChild = new OnlineErrorContainer(ex.Message) { ShowInstantly = true };
        }
    }
}

public abstract partial class NavigatorPage : CompositeDrawable
{
    public abstract string Path { get; }
}

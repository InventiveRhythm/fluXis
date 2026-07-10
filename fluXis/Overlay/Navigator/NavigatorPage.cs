using System;
using System.Threading.Tasks;
using fluXis.Online.Drawables;
using fluXis.Online.Fluxel;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Logging;

namespace fluXis.Overlay.Navigator;

#nullable enable

[LongRunningLoad]
public abstract partial class NavigatorPage<T> : NavigatorPage
    where T : class
{
    [Resolved]
    protected IAPIClient API { get; private set; } = null!;

    [Resolved]
    protected OnlineNavigator Navigator { get; private set; } = null!;

    [Resolved]
    protected FluXisGame? Game { get; private set; }

    protected virtual float ContentWidth => 0f;
    protected abstract T PullData();
    protected abstract Drawable CreateContent(T data);

    private T? data;

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
            data = PullData();
            setContent();
        }
        catch (Exception ex)
        {
            fail(ex);
        }
    }

    private void setContent()
    {
        if (data is null) return;

        InternalChild = CreateContent(data);
    }

    private void fail(Exception ex)
    {
        Logger.Error(ex, "Failed to load page.");

        InternalChild = new OnlineErrorContainer
        {
            Text = ex.Message,
            ShowInstantly = true,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre
        };
    }

    public override void Refresh(Action complete) => Task.Run(() =>
    {
        try
        {
            data = PullData();
            Schedule(setContent);
        }
        catch (Exception ex)
        {
            Schedule(() => fail(ex));
        }
        finally
        {
            Schedule(complete);
        }
    });

    protected virtual Drawable? CreateBackground(T data) => null;
    public sealed override Drawable? CreateBackground() => data is null ? null : CreateBackground(data);
}

public abstract partial class NavigatorPage : CompositeDrawable
{
    public abstract string Path { get; }

    public virtual Drawable? CreateBackground() => null;

    public abstract void Refresh(Action complete);
}

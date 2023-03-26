using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Game.Overlay.Notification;

public partial class NotificationContainer : FillFlowContainer<Notification>
{
    private readonly List<Notification> notifications = new();

    private ISampleStore samples;

    public NotificationContainer()
    {
        Anchor = Anchor.TopRight;
        Origin = Anchor.TopRight;
        Margin = new MarginPadding { Top = 10, Right = 20 };
        Direction = FillDirection.Vertical;
        AutoSizeAxes = Axes.Y;
        Width = 300;
        Spacing = new Vector2(0, 10);
    }

    [BackgroundDependencyLoader]
    private void load(ISampleStore samples)
    {
        this.samples = samples;
    }

    public void AddNotification(Notification notification)
    {
        notifications.Add(notification);
        Add(notification);
        samples.Get(notification.SampleAppearing)?.Play();
    }

    protected override void Update()
    {
        notifications.Where(x => x.Lifetime <= 0).ForEach(n =>
        {
            n.PopOut().OnComplete(_ => Remove(n, true));
            Schedule(() =>
            {
                notifications.Remove(n);
                samples.Get(n.SampleDisappearing)?.Play();
            });
        });
    }
}

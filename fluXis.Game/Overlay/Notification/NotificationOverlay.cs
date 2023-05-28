using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Overlay.Notification;

public partial class NotificationOverlay : Container
{
    private readonly NotificationContainer notificationContainer;

    [Resolved]
    private Toolbar.Toolbar toolbar { get; set; }

    public NotificationOverlay()
    {
        RelativeSizeAxes = Axes.Both;
        Children = new Drawable[]
        {
            notificationContainer = new NotificationContainer()
        };
    }

    public void AddNotification(Notification notification) => Schedule(() => notificationContainer.AddNotification(notification));

    public void Post(string text) => AddNotification(new SimpleNotification { Text = text });
    public void PostError(string text, float time = 5000) => AddNotification(new ErrorNotification { Text = text, Lifetime = time });

    protected override void Update()
    {
        var expectedY = toolbar.Y + toolbar.Height;

        if (Padding.Top != expectedY)
            Padding = new MarginPadding { Top = expectedY };
    }
}

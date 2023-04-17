using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Overlay.Notification;

public partial class NotificationOverlay : Container
{
    private readonly NotificationContainer notificationContainer;

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
    public void PostError(string text) => AddNotification(new ErrorNotification { Text = text });
}
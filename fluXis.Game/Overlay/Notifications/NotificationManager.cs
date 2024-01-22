using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Overlay.Notifications.Floating;
using osu.Framework.Development;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Logging;

namespace fluXis.Game.Overlay.Notifications;

public partial class NotificationManager : Component
{
    public FloatingNotificationContainer Floating { get; set; }

    public void Add(INotificationData notification)
    {
        if (!ThreadSafety.IsUpdateThread)
        {
            Scheduler.Add(() => Add(notification));
            return;
        }

        Logger.Log($"Sending notification: {notification}");
        Floating?.Add(notification.CreateFloating());
    }

    public void SendSmallText(string text) => SendSmallText(text, FontAwesome6.Solid.Info);

    public void SendSmallText(string text, IconUsage icon)
    {
        if (!ThreadSafety.IsUpdateThread)
        {
            Scheduler.Add(() => SendSmallText(text, icon));
            return;
        }

        Logger.Log($"Sending small notification: {text}");
        Floating?.Add(new SmallFloatingTextNotification
        {
            Text = text,
            Icon = icon
        });
    }

    public void SendText(string text, string subtext = "") => SendText(text, subtext, FontAwesome6.Solid.Info);

    public void SendText(string text, string subtext, IconUsage icon)
    {
        if (!ThreadSafety.IsUpdateThread)
        {
            Scheduler.Add(() => SendText(text, subtext, icon));
            return;
        }

        Logger.Log($"Sending notification: {text}");
        Floating?.Add(new FloatingTextNotification
        {
            Text = text,
            SubText = subtext,
            Icon = icon
        });
    }

    public void SendError(string text, string subtext = "") => SendError(text, subtext, FontAwesome6.Solid.XMark);

    public void SendError(string text, string subtext, IconUsage icon)
    {
        if (!ThreadSafety.IsUpdateThread)
        {
            Scheduler.Add(() => SendError(text, subtext, icon));
            return;
        }

        Logger.Log($"Sending error notification: {text}");
        Floating?.Add(new FloatingTextNotification
        {
            Text = text,
            SubText = subtext,
            Icon = icon,
            BackgroundColor = FluXisColors.ButtonRed,
            SampleAppearing = "UI/Notifications/error.mp3",
            Lifetime = 10000
        });
    }
}

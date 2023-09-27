using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Overlay.Notifications.Floating;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Logging;

namespace fluXis.Game.Overlay.Notifications;

public partial class NotificationManager : Component
{
    public FloatingNotificationContainer Floating { get; set; }

    public void Add(INotificationData notification)
    {
        Logger.Log($"Sending notification: {notification}");
        Floating?.Add(notification.CreateFloating());
    }

    public void SendText(string text, string subtext = "") => SendText(text, subtext, FontAwesome.Solid.Info);

    public void SendText(string text, string subtext, IconUsage icon)
    {
        Logger.Log($"Sending notification: {text}");
        Floating?.Add(new FloatingTextNotification
        {
            Text = text,
            SubText = subtext,
            Icon = icon
        });
    }

    public void SendError(string text, string subtext = "") => SendError(text, subtext, FontAwesome.Solid.Times);

    public void SendError(string text, string subtext, IconUsage icon)
    {
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

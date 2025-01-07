using System;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Overlay.Notifications.Floating;
using fluXis.Overlay.Notifications.Tasks;
using osu.Framework.Development;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Logging;

namespace fluXis.Overlay.Notifications;

public partial class NotificationManager : Component
{
    public FloatingNotificationContainer Floating { get; set; }
    public TaskNotificationContainer Tasks { get; set; }

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

    public void AddTask(TaskNotificationData notification)
    {
        if (!ThreadSafety.IsUpdateThread)
        {
            Scheduler.Add(() => AddTask(notification));
            return;
        }

        Logger.Log($"Sending task notification: {notification}");
        Tasks?.Add(notification.Create());
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

    public void SendText(string text, string subtext = "", Action action = null) => SendText(text, subtext, FontAwesome6.Solid.Info, action);

    public void SendText(string text, string subtext, IconUsage icon, Action action = null)
    {
        if (!ThreadSafety.IsUpdateThread)
        {
            Scheduler.Add(() => SendText(text, subtext, icon, action));
            return;
        }

        Logger.Log($"Sending notification: {text}");
        Floating?.Add(new FloatingTextNotification
        {
            Text = text,
            SubText = subtext,
            Icon = icon,
            Action = action
        });
    }

    public void SendError(string text, string subtext = "", Action action = null) => SendError(text, subtext, FontAwesome6.Solid.XMark, action);

    public void SendError(string text, string subtext, IconUsage icon, Action action = null)
    {
        if (!ThreadSafety.IsUpdateThread)
        {
            Scheduler.Add(() => SendError(text, subtext, icon, action));
            return;
        }

        Logger.Log($"Sending error notification: {text}");
        Floating?.Add(new FloatingTextNotification
        {
            Text = text,
            SubText = subtext,
            Icon = icon,
            AccentColor = FluXisColors.Red,
            SampleAppearing = "UI/Notifications/error.mp3",
            Lifetime = 10000,
            Action = action
        });
    }
}

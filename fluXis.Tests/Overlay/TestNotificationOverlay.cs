using fluXis.Graphics.Sprites.Icons;
using fluXis.Overlay.Notifications;
using fluXis.Overlay.Notifications.Tasks;
using osu.Framework.Allocation;

namespace fluXis.Tests.Overlay;

public partial class TestNotificationOverlay : FluXisTestScene
{
    [BackgroundDependencyLoader]
    private void load(NotificationManager notifications)
    {
        Add(notifications.Floating = new FloatingNotificationContainer());
        Add(notifications.Tasks = new TaskNotificationContainer());

        int count = 0;

        AddStep("Send notification", () => notifications.SendText("This is a test notification"));
        AddStep("Send small notification", () => notifications.SendSmallText("This is a small notification", FontAwesome6.Solid.Bars));
        AddStep("Send counting notification", () => notifications.SendText("This is a test notification", $"Count: {++count}", FontAwesome6.Solid.Circle));
        AddStep("Send error notification", () => notifications.SendError("This is a test error notification"));
    }
}

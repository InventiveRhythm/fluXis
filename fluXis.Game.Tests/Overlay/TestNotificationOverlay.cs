using fluXis.Game.Overlay.Notification;
using osu.Framework.Allocation;

namespace fluXis.Game.Tests.Overlay;

public partial class TestNotificationOverlay : FluXisTestScene
{
    [BackgroundDependencyLoader]
    private void load(NotificationOverlay notifications)
    {
        Add(notifications);

        AddStep("Post notification", () => notifications.Post("This is a test notification"));
        AddStep("Post error notification", () => notifications.PostError("This is a test error notification"));
    }
}

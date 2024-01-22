using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Overlay.Notifications;
using fluXis.Game.Overlay.Notifications.Types.Loading;
using osu.Framework.Allocation;

namespace fluXis.Game.Tests.Overlay;

public partial class TestNotificationOverlay : FluXisTestScene
{
    [BackgroundDependencyLoader]
    private void load(NotificationManager notifications)
    {
        Add(notifications.Floating = new FloatingNotificationContainer());

        int count = 0;

        AddStep("Send notification", () => notifications.SendText("This is a test notification"));
        AddStep("Send counting notification", () => notifications.SendText("This is a test notification", $"Count: {++count}", FontAwesome6.Solid.Circle));
        AddStep("Send error notification", () => notifications.SendError("This is a test error notification"));

        var loading = new LoadingNotificationData();
        AddStep("Send loading notification", () => notifications.Add(loading));
        AddSliderStep("Set loading progress", 0f, 1f, 0f, v => loading.Progress = v);
        AddStep("Start loading notification", () => loading.State = LoadingState.Working);
        AddStep("Fail loading notification", () => loading.State = LoadingState.Failed);
        AddStep("Finish loading notification", () => loading.State = LoadingState.Complete);
    }
}

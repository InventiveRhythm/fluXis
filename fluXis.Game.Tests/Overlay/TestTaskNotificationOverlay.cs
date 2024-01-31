using fluXis.Game.Overlay.Notifications;
using fluXis.Game.Overlay.Notifications.Tasks;
using osu.Framework.Allocation;

namespace fluXis.Game.Tests.Overlay;

public partial class TestTaskNotificationOverlay : FluXisTestScene
{
    [BackgroundDependencyLoader]
    private void load(NotificationManager notifications)
    {
        Add(notifications.Tasks = new TaskNotificationContainer());

        var task = new TaskNotificationData();
        AddStep("Send task notification", () => notifications.AddTask(task));
        AddSliderStep("Set task progress", 0f, 1f, 0f, v => task.Progress = v);
        AddStep("Start task notification", () => task.State = LoadingState.Working);
        AddStep("Fail task notification", () => task.State = LoadingState.Failed);
        AddStep("Finish task notification", () => task.State = LoadingState.Complete);
    }
}

using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Game.Overlay.Notifications.Tasks;

public partial class TaskNotificationContainer : FillFlowContainer<TaskNotification>
{
    public TaskNotificationContainer()
    {
        AutoSizeAxes = Axes.Both;
        Margin = new MarginPadding(20);
        Spacing = new Vector2(10);
        Direction = FillDirection.Vertical;
        Anchor = Origin = Anchor.BottomLeft;
    }
}

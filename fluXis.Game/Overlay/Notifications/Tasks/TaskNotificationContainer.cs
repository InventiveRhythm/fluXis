using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Overlay.Notifications.Tasks;

public partial class TaskNotificationContainer : Container<TaskNotification>
{
    public TaskNotificationContainer()
    {
        AutoSizeAxes = Axes.Both;
        Margin = new MarginPadding(20);
        Anchor = Origin = Anchor.BottomLeft;
    }

    protected override void Update()
    {
        base.Update();

        var y = 0;

        foreach (var child in Children)
        {
            child.TargetY = y;
            y += 90;
        }
    }
}

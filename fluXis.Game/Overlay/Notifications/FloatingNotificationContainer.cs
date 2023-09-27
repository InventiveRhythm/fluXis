using fluXis.Game.Overlay.Notifications.Floating;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Game.Overlay.Notifications;

public partial class FloatingNotificationContainer : FillFlowContainer<FloatingNotification>
{
    public FloatingNotificationContainer()
    {
        AutoSizeAxes = Axes.Both;
        Padding = new MarginPadding(10);
        Spacing = new Vector2(10);
        Direction = FillDirection.Vertical;
        Anchor = Origin = Anchor.TopCentre;
    }
}

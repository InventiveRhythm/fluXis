using System.Linq;
using fluXis.Overlay.Notifications.Floating;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Overlay.Notifications;

public partial class FloatingNotificationContainer : Container<FloatingNotification>
{
    public FloatingNotificationContainer()
    {
        AutoSizeAxes = Axes.Both;
        Padding = new MarginPadding(10);
        Anchor = Origin = Anchor.TopCentre;
    }

    protected override void Update()
    {
        base.Update();

        const float spacing = 12f;
        var y = Children.Sum(child => child.DrawHeight + spacing);

        foreach (var child in Children)
        {
            child.TargetY = y;
            y -= child.DrawHeight + spacing;
        }
    }
}

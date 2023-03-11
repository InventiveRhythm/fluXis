using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Overlay.Notification;

public partial class NotificationContainer : FillFlowContainer<Notification>
{
    public NotificationContainer()
    {
        Anchor = Anchor.TopRight;
        Origin = Anchor.TopRight;
        Margin = new MarginPadding { Top = 10, Right = 20 };
        Direction = FillDirection.Vertical;
        AutoSizeAxes = Axes.Both;
    }

    public void AddNotification(Notification notification)
    {
        Add(notification);
    }

    protected override void Update()
    {
        while (Children.Count > 0 && Children[0].Lifetime <= 0 && !Children[0].FadeOut)
        {
            var child = Children[0];
            child.FadeOut = true;
            child.Container.FadeOut(200).MoveToX(40, 200, Easing.InQuint).OnComplete(_ => child.ScaleTo(0, 100).OnComplete(_ => Remove(child, true)));
        }

        base.Update();
    }
}

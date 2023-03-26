using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Overlay.Notification;

public partial class ErrorNotification : SimpleNotification
{
    public override string SampleAppearing => "UI/Notifications/error.mp3";

    public ErrorNotification()
    {
        Background.Add(new Box
        {
            RelativeSizeAxes = Axes.Both,
            Colour = Colour4.Red,
            Alpha = .25f
        });
    }
}

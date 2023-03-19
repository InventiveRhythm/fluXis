using fluXis.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Game.Overlay.Notification;

public partial class SimpleNotification : Notification
{
    public string Text { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        Content.Add(new FluXisTextFlow
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Text = Text
        });
    }
}

using fluXis.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Game.Overlay.Notification;

public partial class SimpleNotification : Notification
{
    public string Text
    {
        get => text;
        set
        {
            text = value;
            if (textFlow != null) textFlow.Text = text;
        }
    }

    private string text;
    private FluXisTextFlow textFlow;

    [BackgroundDependencyLoader]
    private void load()
    {
        Content.Add(textFlow = new FluXisTextFlow
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Text = Text
        });
    }
}

using fluXis.Game.Graphics.UserInterface.Text;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osuTK;

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

    protected virtual IconUsage Icon => FontAwesome.Solid.CheckCircle;

    private string text;
    private FluXisTextFlow textFlow;

    [BackgroundDependencyLoader]
    private void load()
    {
        IconContainer.Add(new SpriteIcon
        {
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Icon = Icon,
            Size = new Vector2(20)
        });

        Content.Add(textFlow = new FluXisTextFlow
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Text = Text
        });
    }
}

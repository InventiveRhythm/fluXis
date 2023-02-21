using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;

namespace fluXis.Game.Overlay.Settings.UI;

public partial class SettingsItem : CircularContainer
{
    private readonly Box background;

    protected override Container Content { get; }

    public SettingsItem()
    {
        RelativeSizeAxes = Axes.X;
        Masking = true;
        Height = 60;

        AddInternal(Content = new Container
        {
            RelativeSizeAxes = Axes.Both,
            Padding = new MarginPadding(20)
        });

        AddInternal(background = new Box
        {
            RelativeSizeAxes = Axes.Both,
            Colour = Colour4.White,
            Alpha = 0
        });
    }

    public override void Add(Drawable drawable)
    {
        Content.Add(drawable);
    }

    protected override bool OnHover(HoverEvent e)
    {
        background.FadeTo(.2f, 200);

        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        background.FadeTo(0, 200);
    }
}

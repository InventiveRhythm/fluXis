using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;

namespace fluXis.Game.Overlay.Settings.UI;

public partial class SettingsItem : Container
{
    private Box background;
    private Container backgroundContainer;

    public string Label { get; init; } = string.Empty;

    protected override Container<Drawable> Content { get; } = new Container
    {
        RelativeSizeAxes = Axes.Both,
        Padding = new MarginPadding { Horizontal = 20 }
    };

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 60;

        InternalChildren = new Drawable[]
        {
            backgroundContainer = new Container
            {
                Masking = true,
                RelativeSizeAxes = Axes.Both,
                Child = background = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Colour4.White,
                    Alpha = 0
                }
            },
            Content
        };
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

    protected override void Update()
    {
        float halfHeight = DrawHeight / 2;
        if (halfHeight > 30) halfHeight = 30;

        if (backgroundContainer.CornerRadius != halfHeight)
            backgroundContainer.CornerRadius = halfHeight;
    }
}

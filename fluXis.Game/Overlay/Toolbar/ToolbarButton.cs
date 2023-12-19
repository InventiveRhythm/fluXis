using fluXis.Game.Audio;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Text;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Overlay.Mouse;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Overlay.Toolbar;

public partial class ToolbarButton : ClickableContainer, IHasDrawableTooltip
{
    public IconUsage Icon { get; init; } = FontAwesome.Solid.Question;
    public string Title { get; init; }
    public string Description { get; init; }
    public bool RequireLogin { get; init; }

    [Resolved]
    private UISamples samples { get; set; }

    [Resolved]
    private Fluxel fluxel { get; set; }

    private Box hover;
    private Box flash;

    [BackgroundDependencyLoader]
    private void load()
    {
        Size = new Vector2(40);
        Margin = new MarginPadding(5);
        Anchor = Anchor.CentreLeft;
        Origin = Anchor.CentreLeft;
        CornerRadius = 5;
        Masking = true;

        Children = new Drawable[]
        {
            hover = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Alpha = 0
            },
            flash = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Alpha = 0
            },
            new SpriteIcon
            {
                Icon = Icon,
                Size = new Vector2(20),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        Enabled.BindValueChanged(e => this.FadeTo(e.NewValue ? 1 : .2f, 200), true);

        if (!RequireLogin) return;

        fluxel.OnUserChanged += _ => updateState();
        updateState();
    }

    private void updateState() => Enabled.Value = fluxel.LoggedInUser != null;

    protected override bool OnHover(HoverEvent e)
    {
        if (!Enabled.Value) return true;

        hover.FadeTo(.2f, 50);
        samples.Hover();
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        hover.FadeOut(200);
    }

    protected override bool OnClick(ClickEvent e)
    {
        if (Enabled.Value)
            flash.FadeOutFromOne(1000, Easing.OutQuint);

        samples.Click(!Enabled.Value);
        return base.OnClick(e);
    }

    public Drawable GetTooltip()
    {
        if (!Enabled.Value)
        {
            return new FluXisSpriteText
            {
                Text = "Log in to use this feature.",
                Margin = new MarginPadding { Horizontal = 10, Vertical = 6 }
            };
        }

        return new FillFlowContainer
        {
            AutoSizeAxes = Axes.Both,
            Direction = FillDirection.Vertical,
            Margin = new MarginPadding { Horizontal = 10, Vertical = 6 },
            Children = new Drawable[]
            {
                new FillFlowContainer
                {
                    AutoSizeAxes = Axes.Both,
                    Direction = FillDirection.Horizontal,
                    Spacing = new Vector2(5),
                    Children = new Drawable[]
                    {
                        new SpriteIcon
                        {
                            Icon = Icon,
                            Size = new Vector2(16),
                            Margin = new MarginPadding(4)
                        },
                        new FluXisSpriteText
                        {
                            Text = Title,
                            FontSize = 24
                        }
                    }
                },
                new FluXisTextFlow
                {
                    AutoSizeAxes = Axes.Both,
                    Text = Description,
                    FontSize = 18
                }
            }
        };
    }
}

using fluXis.Game.Audio;
using fluXis.Game.Database;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Text;
using fluXis.Game.Input;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Overlay.Mouse;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Overlay.Toolbar;

public partial class ToolbarButton : ClickableContainer, IHasDrawableTooltip, IKeyBindingHandler<FluXisGlobalKeybind>
{
    public string TooltipTitle { get; init; }
    public string TooltipSub { get; init; }
    public IconUsage Icon { get; init; } = FontAwesome.Solid.Question;
    public FluXisGlobalKeybind Keybind { get; init; } = FluXisGlobalKeybind.None;
    public bool RequireLogin { get; init; }

    [Resolved]
    private UISamples samples { get; set; }

    [Resolved]
    private Fluxel fluxel { get; set; }

    [Resolved]
    private FluXisRealm realm { get; set; }

    [Resolved]
    private ReadableKeyCombinationProvider keyCombinationProvider { get; set; }

    private Circle line;
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
            new Container
            {
                RelativeSizeAxes = Axes.X,
                Height = 10,
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                Colour = FluXisColors.Highlight,
                Padding = new MarginPadding(3),
                Child = line = new Circle
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Width = 0
                }
            },
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

        fluxel.OnUserChanged += _ => Schedule(updateState);
        updateState();
    }

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

    public bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        if (e.Action != Keybind) return false;

        TriggerClick();
        return true;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisGlobalKeybind> e) { }

    protected void SetLineState(bool state) => line.ResizeWidthTo(state ? 1 : 0, 400, Easing.OutQuint);

    private void updateState() => Enabled.Value = fluxel.LoggedInUser != null;

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

        var title = TooltipTitle;

        if (Keybind != FluXisGlobalKeybind.None)
            title += $" ({keyCombinationProvider.GetReadableString(InputUtils.GetBindingFor(Keybind, realm).KeyCombination)})";

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
                            Text = title,
                            FontSize = 24
                        }
                    }
                },
                new FluXisTextFlow
                {
                    AutoSizeAxes = Axes.Both,
                    Text = TooltipSub,
                    FontSize = 18
                }
            }
        };
    }
}

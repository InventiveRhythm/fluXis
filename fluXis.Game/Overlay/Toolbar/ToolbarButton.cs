using fluXis.Game.Audio;
using fluXis.Game.Database;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Text;
using fluXis.Game.Input;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Overlay.Mouse;
using fluXis.Game.Utils;
using fluXis.Shared.Components.Users;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Game.Overlay.Toolbar;

public partial class ToolbarButton : ClickableContainer, IHasCustomTooltip<ToolbarButton>, IKeyBindingHandler<FluXisGlobalKeybind>
{
    public string TooltipTitle { get; init; }
    public string TooltipSub { get; init; }
    public IconUsage Icon { get; init; } = FontAwesome6.Solid.Question;
    public FluXisGlobalKeybind Keybind { get; init; } = FluXisGlobalKeybind.None;
    public bool RequireLogin { get; init; }

    public ToolbarButton TooltipContent => this;
    private string keybindText => Keybind != FluXisGlobalKeybind.None ? $" ({keyCombinationProvider.GetReadableString(InputUtils.GetBindingFor(Keybind, realm).KeyCombination)})" : string.Empty;

    [Resolved]
    private UISamples samples { get; set; }

    [Resolved]
    private IAPIClient api { get; set; }

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

        api.User.BindValueChanged(updateState, true);
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

    private void updateState(ValueChangedEvent<APIUser> valueChangedEvent)
        => Schedule(() => Enabled.Value = api.User.Value != null);

    public ITooltip<ToolbarButton> GetCustomTooltip() => new ToolbarButtonTooltip();

    private partial class ToolbarButtonTooltip : CustomTooltipContainer<ToolbarButton>
    {
        private SpriteIcon icon { get; }
        private FluXisSpriteText title { get; }
        private FluXisTextFlow description { get; }

        private LocalisableString currentDesc;

        public ToolbarButtonTooltip()
        {
            Child = new FillFlowContainer
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
                            icon = new SpriteIcon
                            {
                                Size = new Vector2(16),
                                Margin = new MarginPadding(4)
                            },
                            title = new FluXisSpriteText
                            {
                                FontSize = 24
                            }
                        }
                    },
                    description = new FluXisTextFlow
                    {
                        AutoSizeAxes = Axes.Both,
                        FontSize = 18
                    }
                }
            };
        }

        public override void SetContent(ToolbarButton content)
        {
            if (!content.Enabled.Value)
            {
                icon.Alpha = 0;
                title.Text = "Log in to use this feature.";
                description.Alpha = 0;
                return;
            }

            icon.Alpha = 1;
            description.Alpha = 1;

            icon.Icon = content.Icon;
            title.Text = content.TooltipTitle + content.keybindText;

            if (currentDesc != content.TooltipSub)
                description.Text = currentDesc = content.TooltipSub;
        }
    }
}

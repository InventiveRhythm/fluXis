using fluXis.Audio;
using fluXis.Database;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Text;
using fluXis.Input;
using fluXis.Online.API.Models.Users;
using fluXis.Online.Fluxel;
using fluXis.Overlay.Mouse;
using fluXis.Utils;
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

namespace fluXis.Overlay.Toolbar;

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

    public Bindable<bool> ShowPulse { get; init; } = new();

    private Circle pulse;
    private Circle line;
    private Container content;
    private HoverLayer hover;
    private FlashLayer flash;

    [BackgroundDependencyLoader]
    private void load()
    {
        Size = new Vector2(40);
        Margin = new MarginPadding(5);
        Anchor = Anchor.CentreLeft;
        Origin = Anchor.CentreLeft;

        InternalChildren = new Drawable[]
        {
            pulse = new Circle
            {
                Colour = FluXisColors.Highlight,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Alpha = 0
            },
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
            content = new Container
            {
                RelativeSizeAxes = Axes.Both,
                CornerRadius = 5,
                Masking = true,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Children = new Drawable[]
                {
                    hover = new HoverLayer(),
                    flash = new FlashLayer(),
                    new FluXisSpriteIcon
                    {
                        Icon = Icon,
                        Size = new Vector2(20),
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre
                    }
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        ShowPulse.BindValueChanged(v =>
        {
            if (v.NewValue)
            {
                pulse.Delay(800).ResizeTo(0)
                     .ResizeTo(64, 2000, Easing.OutQuint)
                     .FadeOutFromOne(1200).Loop();
            }
            else
            {
                pulse.ClearTransforms();
                pulse.Hide();
            }
        }, true);

        Enabled.BindValueChanged(e => this.FadeTo(e.NewValue ? 1 : .2f, 200), true);

        if (!RequireLogin) return;

        api.User.BindValueChanged(updateState, true);
    }

    protected override bool OnHover(HoverEvent e)
    {
        if (!Enabled.Value) return true;

        hover.Show();
        samples.Hover();
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        hover.Hide();
    }

    protected override bool OnClick(ClickEvent e)
    {
        if (Enabled.Value)
            flash.Show();

        samples.Click(!Enabled.Value);
        return base.OnClick(e);
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        content.ScaleTo(.9f, 1000, Easing.OutQuint);
        return true;
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        content.ScaleTo(1, 1000, Easing.OutElastic);
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        if (e.Action != Keybind || e.Repeat) return false;

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
                            icon = new FluXisSpriteIcon
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

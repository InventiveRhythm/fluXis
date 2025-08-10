using fluXis.Audio;
using fluXis.Graphics;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Interaction;
using fluXis.Graphics.UserInterface.Panel;
using fluXis.Graphics.UserInterface.Panel.Types;
using fluXis.Input;
using fluXis.Online.API.Models.Users;
using fluXis.Online.Drawables.Images;
using fluXis.Online.Fluxel;
using fluXis.Overlay.User;
using fluXis.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Overlay.Toolbar;

public partial class ToolbarProfile : VisibilityContainer, IHasTooltip, IKeyBindingHandler<FluXisGlobalKeybind>
{
    public LocalisableString TooltipText => loadingContainer.Alpha > 0 ? "Connecting..." : "";

    [Resolved]
    private UserProfileOverlay profile { get; set; }

    [Resolved]
    private IAPIClient api { get; set; }

    [Resolved]
    private PanelContainer panels { get; set; }

    [Resolved]
    private UISamples samples { get; set; }

    private Container container;
    private DrawableAvatar avatar;
    private Container loadingContainer;
    private HoverLayer hover;
    private FlashLayer flash;
    private SpriteIcon arrow;

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Both;

        var user = api.User.Value;

        Children = new Drawable[]
        {
            container = new Container
            {
                Width = 60,
                Height = 70,
                Y = -10,
                CornerRadius = 10,
                Masking = true,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Theme.Background4
                    },
                    hover = new HoverLayer(),
                    flash = new FlashLayer(),
                    new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Padding = new MarginPadding(10) { Top = 20 },
                        Children = new Drawable[]
                        {
                            new Container
                            {
                                Size = new Vector2(40),
                                Children = new Drawable[]
                                {
                                    new LoadWrapper<DrawableAvatar>
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        CornerRadius = 5,
                                        Masking = true,
                                        LoadContent = () => avatar = new DrawableAvatar(user)
                                        {
                                            RelativeSizeAxes = Axes.Both,
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre
                                        }
                                    },
                                    loadingContainer = new Container
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Alpha = 0,
                                        Children = new Drawable[]
                                        {
                                            new Box
                                            {
                                                RelativeSizeAxes = Axes.Both,
                                                Colour = Theme.Background4,
                                                Alpha = .5f
                                            },
                                            new LoadingIcon
                                            {
                                                Anchor = Anchor.Centre,
                                                Origin = Anchor.Centre,
                                                Size = new Vector2(20)
                                            }
                                        }
                                    }
                                }
                            },
                            arrow = new FluXisSpriteIcon
                            {
                                Icon = FontAwesome6.Solid.AngleDown,
                                Size = new Vector2(10),
                                Y = 40,
                                Anchor = Anchor.TopCentre,
                                Origin = Anchor.TopCentre,
                                Alpha = 0
                            }
                        }
                    }
                }
            }
        };

        api.User.BindValueChanged(updateUser);
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        api.Status.BindValueChanged(updateStatus, true);
    }

    protected override void PopIn() => container.MoveToY(-10, Styling.TRANSITION_MOVE, Easing.OutQuint);
    protected override void PopOut() => container.MoveToY(-20, Styling.TRANSITION_MOVE, Easing.OutQuint);

    private void updateStatus(ValueChangedEvent<ConnectionStatus> e) => Scheduler.ScheduleIfNeeded(() =>
    {
        switch (e.NewValue)
        {
            case ConnectionStatus.Online:
                loadingContainer.FadeOut(200);
                break;

            case ConnectionStatus.Reconnecting:
            case ConnectionStatus.Connecting:
                loadingContainer.FadeIn(200);
                break;
        }
    });

    private void updateUser(ValueChangedEvent<APIUser> e)
    {
        avatar.UpdateUser(e.NewValue);
    }

    protected override bool OnClick(ClickEvent e)
    {
        flash.Show();
        samples.Click();

        if (!api.CanUseOnline || api.User.Value == null)
        {
            panels.Content = new SingleButtonPanel(FontAwesome6.Solid.PlugCircleXMark, "You must be online to use this feature.", "Restart fluXis to connect to the online services.", "Okay");
            return true;
        }

        if (profile.State.Value == Visibility.Visible)
            profile.Hide();
        else
            profile.ShowUser(api.User.Value.ID);

        return true;
    }

    protected override bool OnHover(HoverEvent e)
    {
        hover.Show();
        container.ResizeHeightTo(80, 400, Easing.OutQuint);
        arrow.FadeIn(200).MoveToY(45, 400, Easing.OutQuint);
        samples.Hover();
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        hover.Hide();
        container.ResizeHeightTo(70, 400, Easing.OutQuint);
        arrow.FadeOut(200).MoveToY(40, 400, Easing.OutQuint);
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        if (e.Repeat)
            return false;

        switch (e.Action)
        {
            case FluXisGlobalKeybind.ToggleProfile:
                TriggerClick();
                return true;
        }

        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisGlobalKeybind> e) { }
}

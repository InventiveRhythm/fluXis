using fluXis.Game.Audio;
using fluXis.Game.Graphics.Drawables;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface;
using fluXis.Game.Graphics.UserInterface.Buttons;
using fluXis.Game.Graphics.UserInterface.Buttons.Presets;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Panel;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Overlay.Login;
using fluXis.Game.Overlay.User;
using fluXis.Shared.Components.Users;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osuTK;
using osuTK.Input;

namespace fluXis.Game.Overlay.Toolbar;

public partial class ToolbarProfile : Container, IHasTooltip
{
    public LocalisableString TooltipText => loadingContainer.Alpha > 0 ? "Connecting..." : "";

    [Resolved]
    private UserProfileOverlay profile { get; set; }

    [Resolved]
    private LoginOverlay loginOverlay { get; set; }

    [Resolved]
    private FluxelClient fluxel { get; set; }

    [Resolved]
    private PanelContainer panels { get; set; }

    [Resolved]
    private UISamples samples { get; set; }

    private Container container;
    private Container avatarContainer;
    private DrawableAvatar avatar;
    private Container loadingContainer;
    private Box hover;
    private Box flash;
    private SpriteIcon arrow;

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Both;

        APIUserShort user = fluxel.LoggedInUser;

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
                        Colour = FluXisColors.Background4
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
                                    avatarContainer = new Container
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        CornerRadius = 5,
                                        Masking = true
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
                                                Colour = FluXisColors.Background4,
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
                            arrow = new SpriteIcon
                            {
                                Icon = FontAwesome6.Solid.ChevronDown,
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

        LoadComponentAsync(avatar = new DrawableAvatar(user)
        {
            RelativeSizeAxes = Axes.Both,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre
        }, avatarContainer.Add);

        fluxel.OnUserChanged += updateUser;
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        fluxel.OnStatusChanged += status =>
        {
            Schedule(() =>
            {
                switch (status)
                {
                    case ConnectionStatus.Offline:
                    case ConnectionStatus.Online:
                    case ConnectionStatus.Closed:
                        loadingContainer.FadeOut(200);
                        break;

                    case ConnectionStatus.Reconnecting:
                    case ConnectionStatus.Connecting:
                    case ConnectionStatus.Failing:
                        loadingContainer.FadeIn(200);
                        break;
                }
            });
        };
    }

    private void updateUser(APIUserShort user)
    {
        avatar.UpdateUser(user);
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        if (e.Button != MouseButton.Right) return false;

        panels.Content = new ButtonPanel
        {
            Icon = FontAwesome6.Solid.DoorOpen,
            Text = "Are you sure you want to log out?",
            Buttons = new ButtonData[]
            {
                new DangerButtonData(ButtonPanel.COMMON_CONFIRM, () => fluxel.Logout()),
                new CancelButtonData(ButtonPanel.COMMON_CANCEL)
            }
        };

        return true;
    }

    protected override bool OnClick(ClickEvent e)
    {
        flash.FadeOutFromOne(1000, Easing.OutQuint);
        samples.Click();

        if (fluxel.LoggedInUser == null)
            loginOverlay.Show();
        else
        {
            if (profile.State.Value == Visibility.Visible)
                profile.Hide();
            else
                profile.ShowUser(fluxel.LoggedInUser.ID);
        }

        return true;
    }

    protected override bool OnHover(HoverEvent e)
    {
        hover.FadeTo(.2f, 50);
        container.ResizeHeightTo(80, 400, Easing.OutQuint);
        arrow.FadeIn(200).MoveToY(45, 400, Easing.OutQuint);
        samples.Hover();
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        hover.FadeOut(200);
        container.ResizeHeightTo(70, 400, Easing.OutQuint);
        arrow.FadeOut(200).MoveToY(40, 400, Easing.OutQuint);
    }
}

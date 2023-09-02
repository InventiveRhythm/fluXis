using fluXis.Game.Audio;
using fluXis.Game.Graphics.Drawables;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Online.API.Users;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Overlay.Login;
using fluXis.Game.Overlay.Profile;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Overlay.Toolbar;

public partial class ToolbarProfile : Container
{
    [Resolved]
    private ProfileOverlay profileOverlay { get; set; }

    [Resolved]
    private LoginOverlay loginOverlay { get; set; }

    [Resolved]
    private Fluxel fluxel { get; set; }

    [Resolved]
    private UISamples samples { get; set; }

    private Container container;
    private Container avatarContainer;
    private DrawableAvatar avatar;
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
                            avatarContainer = new Container
                            {
                                Size = new Vector2(40),
                                CornerRadius = 5,
                                Masking = true
                            },
                            arrow = new SpriteIcon
                            {
                                Icon = FontAwesome.Solid.ChevronDown,
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
            RelativeSizeAxes = Axes.Both
        }, avatarContainer.Add);

        fluxel.OnUserLoggedIn += updateUser;
    }

    private void updateUser(APIUserShort user)
    {
        if (user == null) return;

        avatar.UpdateUser(user);
    }

    protected override bool OnClick(ClickEvent e)
    {
        flash.FadeOutFromOne(1000, Easing.OutQuint);
        samples.Click();

        if (fluxel.LoggedInUser == null)
            loginOverlay.Show();
        else
        {
            profileOverlay.UpdateUser(fluxel.LoggedInUser.ID);
            profileOverlay.ToggleVisibility();
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

using fluXis.Game.Graphics;
using fluXis.Game.Online.API;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Overlay.Login;
using fluXis.Game.Overlay.Profile;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Overlay.Toolbar;

public partial class ToolbarProfile : Container
{
    [Resolved]
    private ProfileOverlay profileOverlay { get; set; }

    [Resolved]
    private LoginOverlay loginOverlay { get; set; }

    private Container avatarContainer;
    private DrawableAvatar avatar;
    private FluXisSpriteText username;
    private Box background;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Y;
        AutoSizeAxes = Axes.X;
        Padding = new MarginPadding { Horizontal = 10 };

        APIUserShort user = Fluxel.LoggedInUser;

        Children = new Drawable[]
        {
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Shear = new Vector2(0.1f, 0),
                Masking = true,
                CornerRadius = 5,
                Child = background = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0
                }
            },
            username = new FluXisSpriteText
            {
                Text = user?.Username ?? "Not logged in",
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
                Margin = new MarginPadding { Right = 45, Left = 10 }
            },
            new Container
            {
                Size = new Vector2(30),
                Masking = true,
                CornerRadius = 5,
                Shear = new Vector2(0.15f, 0),
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
                Margin = new MarginPadding { Right = 10 },
                Child = avatarContainer = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Scale = new Vector2(1.1f),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Shear = new Vector2(-0.15f, 0)
                }
            }
        };

        LoadComponentAsync(avatar = new DrawableAvatar(user)
        {
            RelativeSizeAxes = Axes.Both,
        }, avatarContainer.Add);

        Fluxel.OnUserLoggedIn += updateUser;
    }

    private void updateUser(APIUserShort user)
    {
        if (user == null) return;

        username.Text = user.Username;
        avatar.UpdateUser(user);
    }

    protected override bool OnClick(ClickEvent e)
    {
        if (Fluxel.LoggedInUser == null)
        {
            loginOverlay.Show();
        }
        else
        {
            profileOverlay.UpdateUser(Fluxel.LoggedInUser.ID);
            profileOverlay.ToggleVisibility();
        }

        return true;
    }

    protected override bool OnHover(HoverEvent e)
    {
        background.FadeTo(.2f, 200);
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        background.FadeOut(200);
    }
}

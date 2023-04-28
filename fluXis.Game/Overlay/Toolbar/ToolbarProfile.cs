using fluXis.Game.Graphics;
using fluXis.Game.Online.API;
using fluXis.Game.Online.Fluxel;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Game.Overlay.Toolbar;

public partial class ToolbarProfile : Container
{
    private Container avatarContainer;
    private DrawableAvatar avatar;
    private SpriteText username;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Y;
        AutoSizeAxes = Axes.X;
        Padding = new MarginPadding { Horizontal = 10 };

        APIUserShort user = Fluxel.LoggedInUser;

        Children = new Drawable[]
        {
            username = new SpriteText
            {
                Text = user?.Username ?? "Not logged in",
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
                Font = FluXisFont.Default(),
                Margin = new MarginPadding { Right = 35 }
            },
            avatarContainer = new Container
            {
                Size = new Vector2(30),
                Masking = true,
                CornerRadius = 5,
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
            }
        };

        LoadComponentAsync(avatar = new DrawableAvatar(user) { RelativeSizeAxes = Axes.Both }, avatarContainer.Add);

        Fluxel.OnUserLoggedIn += updateUser;
    }

    private void updateUser(APIUserShort user)
    {
        if (user == null) return;

        username.Text = user.Username;
        avatar.UpdateUser(user);
    }
}

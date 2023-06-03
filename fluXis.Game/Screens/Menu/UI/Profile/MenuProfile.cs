using fluXis.Game.Graphics;
using fluXis.Game.Online.API;
using fluXis.Game.Online.Fluxel;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Game.Screens.Menu.UI.Profile;

public partial class MenuProfile : Container
{
    private readonly DrawableAvatar avatar;
    private readonly FluXisSpriteText username;

    public MenuProfile()
    {
        Anchor = Anchor.TopRight;
        Origin = Anchor.TopRight;
        AutoSizeAxes = Axes.Both;

        APIUserShort user = Fluxel.LoggedInUser;

        InternalChildren = new Drawable[]
        {
            new Container
            {
                Size = new Vector2(100, 100),
                Masking = true,
                CornerRadius = 10,
                Child = avatar = new DrawableAvatar(user)
                {
                    RelativeSizeAxes = Axes.Both
                },
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight
            },
            username = new FluXisSpriteText
            {
                Text = user?.Username ?? "Not logged in",
                FontSize = 40,
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
                Margin = new MarginPadding { Right = 120 }
            }
        };

        Fluxel.OnUserLoggedIn += updateUser;
    }

    private void updateUser(APIUserShort user)
    {
        if (user == null) return;

        username.Text = user.Username;
        avatar.UpdateUser(user);
    }
}

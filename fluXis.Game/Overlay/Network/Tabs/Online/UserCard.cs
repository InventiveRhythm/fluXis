using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Drawables;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Online.API.Users;
using fluXis.Game.Overlay.Profile;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Overlay.Network.Tabs.Online;

public partial class UserCard : Container
{
    public APIUserShort User { get; }

    [Resolved]
    private ProfileOverlay profile { get; set; }

    private Container bannerContainer;
    private Container avatarContainer;

    public UserCard(APIUserShort user)
    {
        User = user;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Anchor = Origin = Anchor.TopCentre;
        Size = new Vector2(250, 80);
        CornerRadius = 20;
        Masking = true;

        InternalChildren = new Drawable[]
        {
            bannerContainer = new Container
            {
                RelativeSizeAxes = Axes.Both,
            },
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Colour4.Black,
                Alpha = .5f
            },
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.Both,
                Direction = FillDirection.Horizontal,
                Spacing = new Vector2(10),
                Padding = new MarginPadding(10),
                Children = new Drawable[]
                {
                    avatarContainer = new Container
                    {
                        Size = new Vector2(60),
                        CornerRadius = 10,
                        Masking = true,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        EdgeEffect = FluXisStyles.ShadowSmall
                    },
                    new FillFlowContainer
                    {
                        AutoSizeAxes = Axes.Both,
                        Direction = FillDirection.Vertical,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Children = new Drawable[]
                        {
                            new FluXisSpriteText
                            {
                                Text = string.IsNullOrEmpty(User.DisplayName) ? User.Username : User.DisplayName,
                                Colour = FluXisColors.GetNameColor(User.Role),
                                Shadow = true,
                                FontSize = 24
                            },
                            new FluXisSpriteText
                            {
                                Text = string.IsNullOrEmpty(User.DisplayName) ? "" : User.Username,
                                Shadow = true,
                                Colour = FluXisColors.Text2,
                                FontSize = 14
                            },
                            new FluXisSpriteText
                            {
                                Text = User.Role switch
                                {
                                    1 => "Featured Artist",
                                    2 => "Purifier",
                                    3 => "Moderator",
                                    4 => "Admin",
                                    _ => "User"
                                },
                                Colour = FluXisColors.GetRoleColor(User.Role),
                                Shadow = true,
                                FontSize = 16
                            }
                        }
                    }
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        LoadComponentAsync(new DrawableBanner(User)
        {
            RelativeSizeAxes = Axes.Both,
            FillMode = FillMode.Fill,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre
        }, bannerContainer.Add);

        LoadComponentAsync(new DrawableAvatar(User)
        {
            RelativeSizeAxes = Axes.Both,
            FillMode = FillMode.Fill,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre
        }, avatarContainer.Add);
    }

    protected override bool OnClick(ClickEvent e)
    {
        profile.UpdateUser(User.ID);
        profile.Show();
        return true;
    }
}

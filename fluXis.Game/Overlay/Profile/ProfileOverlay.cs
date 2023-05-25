using System.Threading.Tasks;
using fluXis.Game.Graphics;
using fluXis.Game.Online;
using fluXis.Game.Online.API;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Logging;
using osuTK;
using osuTK.Graphics;

namespace fluXis.Game.Overlay.Profile;

public partial class ProfileOverlay : Container
{
    public bool IsVisible { get; private set; }

    private APIUser user = APIUser.DummyUser(-1);

    private Container content;
    private ClickableContainer background;

    private DrawableBanner banner;
    private DrawableAvatar avatar;
    private SpriteText username;
    private Box roleBackground;
    private SpriteText role;
    private FillFlowContainer<SocialChip> socialContainer;
    private AboutMeProfileSection aboutMe;

    private Box loadingBox;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        Alpha = 0;

        InternalChildren = new Drawable[]
        {
            background = new ClickableContainer
            {
                RelativeSizeAxes = Axes.Both,
                Child = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color4.Black,
                    Alpha = 0.25f
                },
                Action = Hide
            },
            content = new Container
            {
                Width = 1200,
                Height = 800,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                CornerRadius = 10,
                Masking = true,
                Scale = new Vector2(0.9f),
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = FluXisColors.Background2
                    },
                    new BasicScrollContainer
                    {
                        ScrollbarVisible = false,
                        RelativeSizeAxes = Axes.Both,
                        Children = new Drawable[]
                        {
                            new Container
                            {
                                RelativeSizeAxes = Axes.X,
                                Height = 400,
                                CornerRadius = 10,
                                Masking = true,
                                Children = new Drawable[]
                                {
                                    banner = new DrawableBanner(user)
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Anchor = Anchor.Centre,
                                        Origin = Anchor.Centre
                                    },
                                    new Box
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Colour = Colour4.Black,
                                        Alpha = 0.4f
                                    },
                                    /*new Box
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Colour = ColourInfo.GradientVertical(FluXisColors.Background2.Opacity(0), FluXisColors.Background2)
                                    },*/
                                    new FillFlowContainer
                                    {
                                        AutoSizeAxes = Axes.Both,
                                        Anchor = Anchor.CentreLeft,
                                        Origin = Anchor.CentreLeft,
                                        Direction = FillDirection.Horizontal,
                                        Margin = new MarginPadding { Left = 40 },
                                        Children = new Drawable[]
                                        {
                                            new Container
                                            {
                                                Size = new Vector2(150),
                                                Anchor = Anchor.CentreLeft,
                                                Origin = Anchor.CentreLeft,
                                                Margin = new MarginPadding { Right = 10 },
                                                CornerRadius = 10,
                                                Masking = true,
                                                Child = avatar = new DrawableAvatar(user)
                                                {
                                                    RelativeSizeAxes = Axes.Both,
                                                    Anchor = Anchor.Centre,
                                                    Origin = Anchor.Centre
                                                }
                                            },
                                            new FillFlowContainer
                                            {
                                                AutoSizeAxes = Axes.Both,
                                                Anchor = Anchor.CentreLeft,
                                                Origin = Anchor.CentreLeft,
                                                Direction = FillDirection.Vertical,
                                                Children = new Drawable[]
                                                {
                                                    username = new SpriteText
                                                    {
                                                        Text = user.Username,
                                                        Font = FluXisFont.Default(45)
                                                    },
                                                    new Container
                                                    {
                                                        AutoSizeAxes = Axes.Both,
                                                        CornerRadius = 5,
                                                        Masking = true,
                                                        Children = new Drawable[]
                                                        {
                                                            roleBackground = new Box
                                                            {
                                                                RelativeSizeAxes = Axes.Both,
                                                                Colour = FluXisColors.GetRoleColor(user.Role)
                                                            },
                                                            role = new SpriteText
                                                            {
                                                                Text = APIUser.GetRole(user.Role),
                                                                Font = FluXisFont.Default(25),
                                                                Margin = new MarginPadding { Horizontal = 5, Vertical = 1 }
                                                            }
                                                        }
                                                    },
                                                    socialContainer = new FillFlowContainer<SocialChip>
                                                    {
                                                        AutoSizeAxes = Axes.Both,
                                                        Direction = FillDirection.Horizontal,
                                                        Spacing = new Vector2(10),
                                                        Margin = new MarginPadding { Top = 10 }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            new FillFlowContainer
                            {
                                AutoSizeAxes = Axes.Y,
                                RelativeSizeAxes = Axes.X,
                                Margin = new MarginPadding { Top = 400 },
                                Padding = new MarginPadding(20),
                                Spacing = new Vector2(20),
                                Direction = FillDirection.Vertical,
                                Children = new Drawable[]
                                {
                                    aboutMe = new AboutMeProfileSection { AboutMe = user.AboutMe },
                                    new ProfileSection { Title = "Recent" },
                                    new ProfileSection { Title = "Top Scores" },
                                    new ProfileSection { Title = "Maps" },
                                    new ProfileSection { Title = "Achievements" }
                                }
                            }
                        }
                    },
                    loadingBox = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Colour4.Black,
                        Alpha = 0.5f
                    }
                }
            }
        };

        updateSocials();
    }

    public void UpdateUser(int id)
    {
        loadingBox.FadeTo(.5f, 200);

        Task.Run(() =>
        {
            var newUser = UserCache.GetUser(id);

            user = newUser ?? APIUser.DummyUser(-1);

            if (username != null) username.Text = user.Username;
            if (aboutMe != null) aboutMe.AboutMe = user.AboutMe;

            var roleColor = FluXisColors.GetRoleColor(user.Role);

            if (role != null)
            {
                role.Text = APIUser.GetRole(user.Role);
                role.Colour = FluXisColors.IsBright(roleColor) ? FluXisColors.TextDark : Colour4.White;
            }

            if (roleBackground != null) roleBackground.Colour = roleColor;

            banner?.UpdateUser(user);
            avatar?.UpdateUser(user);

            Schedule(() =>
            {
                updateSocials();
                loadingBox.FadeOut(200);
            });
        });
    }

    private void updateSocials()
    {
        if (socialContainer == null) return;

        socialContainer.Clear();

        if (user?.Socials == null)
        {
            Logger.Log("User socials are null", LoggingTarget.Runtime, LogLevel.Error);
            return;
        }

        if (!string.IsNullOrEmpty(user.Socials.Twitter))
        {
            socialContainer.Add(new SocialChip
            {
                Type = UserSocialType.Twitter,
                Username = user.Socials.Twitter
            });
        }

        if (!string.IsNullOrEmpty(user.Socials.Youtube))
        {
            socialContainer.Add(new SocialChip
            {
                Type = UserSocialType.Youtube,
                Username = user.Socials.Youtube
            });
        }

        if (!string.IsNullOrEmpty(user.Socials.Twitch))
        {
            socialContainer.Add(new SocialChip
            {
                Type = UserSocialType.Twitch,
                Username = user.Socials.Twitch
            });
        }

        if (!string.IsNullOrEmpty(user.Socials.Discord))
        {
            socialContainer.Add(new SocialChip
            {
                Type = UserSocialType.Discord,
                Username = user.Socials.Discord
            });
        }
    }

    public override void Hide()
    {
        this.FadeOut(200);
        content.ScaleTo(0.9f, 400, Easing.OutQuint);
        IsVisible = false;
    }

    public override void Show()
    {
        this.FadeIn(200);
        content.ScaleTo(1, 400, Easing.OutQuint);
        IsVisible = true;
    }

    public void ToggleVisibility()
    {
        if (IsVisible)
            Hide();
        else
            Show();
    }
}

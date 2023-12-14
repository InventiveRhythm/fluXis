using System;
using System.Threading.Tasks;
using fluXis.Game.Audio;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Graphics.Drawables;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Input;
using fluXis.Game.Online;
using fluXis.Game.Online.API.Models.Users;
using fluXis.Game.Overlay.Profile.Stats;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Logging;
using osuTK;
using osuTK.Graphics;

namespace fluXis.Game.Overlay.Profile;

public partial class ProfileOverlay : Container, IKeyBindingHandler<FluXisGlobalKeybind>
{
    [Resolved]
    private UISamples samples { get; set; }

    private bool isVisible { get; set; }

    private APIUser user = APIUser.DummyUser(-1);

    private ClickableContainer content;

    private DrawableBanner banner;
    private DrawableAvatar avatar;
    private FluXisSpriteText username;
    private FluXisSpriteText lastOnline;
    private FluXisSpriteText role;
    private FillFlowContainer<SocialChip> socialContainer;
    private ProfileStats stats;
    private AboutMeProfileSection aboutMe;

    private Container loadingBox;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        Alpha = 0;

        InternalChildren = new Drawable[]
        {
            new ClickableContainer
            {
                RelativeSizeAxes = Axes.Both,
                Child = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color4.Black,
                    Alpha = .5f
                },
                Action = Hide
            },
            content = new ClickableContainer
            {
                Width = 1350,
                Height = 800,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                CornerRadius = 20,
                Masking = true,
                Scale = new Vector2(0.9f),
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = FluXisColors.Background1
                    },
                    new FluXisScrollContainer
                    {
                        ScrollbarVisible = false,
                        RelativeSizeAxes = Axes.Both,
                        Children = new Drawable[]
                        {
                            new Container
                            {
                                RelativeSizeAxes = Axes.X,
                                Height = 400,
                                CornerRadius = 20,
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
                                                CornerRadius = 20,
                                                Masking = true,
                                                Children = new Drawable[]
                                                {
                                                    avatar = new DrawableAvatar(user)
                                                    {
                                                        RelativeSizeAxes = Axes.Both,
                                                        Anchor = Anchor.Centre,
                                                        Origin = Anchor.Centre
                                                    }
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
                                                    username = new FluXisSpriteText
                                                    {
                                                        Text = user.Username,
                                                        FontSize = 45
                                                    },
                                                    lastOnline = new FluXisSpriteText
                                                    {
                                                        Text = "",
                                                        FontSize = 23
                                                    },
                                                    role = new FluXisSpriteText
                                                    {
                                                        Text = APIUser.GetRole(user.Role),
                                                        Colour = FluXisColors.GetRoleColor(user.Role),
                                                        FontSize = 25
                                                    },
                                                    socialContainer = new FillFlowContainer<SocialChip>
                                                    {
                                                        AutoSizeAxes = Axes.Both,
                                                        Direction = FillDirection.Horizontal,
                                                        Spacing = new Vector2(10)
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
                                    stats = new ProfileStats { User = user },
                                    aboutMe = new AboutMeProfileSection { AboutMe = user.AboutMe },
                                    new ProfileSection { Title = "Recent" },
                                    new ProfileSection { Title = "Top Scores" },
                                    new ProfileSection { Title = "Maps" },
                                    new ProfileSection { Title = "Achievements" }
                                }
                            }
                        }
                    },
                    loadingBox = new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Alpha = 1,
                        Children = new Drawable[]
                        {
                            new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Colour = Colour4.Black,
                                Alpha = 0.5f
                            },
                            new LoadingIcon
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre
                            }
                        }
                    }
                }
            }
        };

        updateSocials();
    }

    public void UpdateUser(int id)
    {
        loadingBox.FadeIn(200);

        Task.Run(() =>
        {
            var newUser = UserCache.GetUser(id, true);

            user = newUser ?? APIUser.DummyUser(-1);

            if (username != null) username.Text = user.Username;
            if (aboutMe != null) aboutMe.AboutMe = user.AboutMe;
            if (stats != null) stats.User = user;

            if (lastOnline != null)
            {
                lastOnline.Text = user.Online
                    ? "Online"
                    : $"Last online {TimeUtils.Ago(DateTimeOffset.FromUnixTimeSeconds(user.LastLogin))}";
            }

            if (role != null)
            {
                role.Text = APIUser.GetRole(user.Role);
                role.Colour = FluXisColors.GetRoleColor(user.Role);
            }

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

        if (!string.IsNullOrEmpty(user.Socials.YouTube))
        {
            socialContainer.Add(new SocialChip
            {
                Type = UserSocialType.Youtube,
                Username = user.Socials.YouTube
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
        isVisible = false;
        samples.Overlay(true);
    }

    public override void Show()
    {
        this.FadeIn(200);
        content.ScaleTo(1, 400, Easing.OutQuint);
        isVisible = true;
        samples.Overlay(false);
    }

    public void ToggleVisibility()
    {
        if (isVisible)
            Hide();
        else
            Show();
    }

    protected override bool OnHover(HoverEvent e) => true;
    protected override bool OnDragStart(DragStartEvent e) => true;
    protected override bool OnKeyDown(KeyDownEvent e) => true;
    protected override bool OnScroll(ScrollEvent e) => true;

    public bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        switch (e.Action)
        {
            case FluXisGlobalKeybind.Back:
                Hide();
                break;
        }

        return true;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisGlobalKeybind> e) { }
}

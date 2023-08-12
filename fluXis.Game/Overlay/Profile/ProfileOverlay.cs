using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using fluXis.Game.Configuration;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Scroll;
using fluXis.Game.Input;
using fluXis.Game.Online;
using fluXis.Game.Online.API;
using fluXis.Game.Online.API.Users;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Overlay.Notification;
using fluXis.Game.Overlay.Profile.Stats;
using fluXis.Game.Screens;
using fluXis.Game.Screens.Import;
using fluXis.Game.Utils;
using Newtonsoft.Json;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.IO.Network;
using osu.Framework.Logging;
using osuTK;
using osuTK.Graphics;

namespace fluXis.Game.Overlay.Profile;

public partial class ProfileOverlay : Container, IKeyBindingHandler<FluXisKeybind>
{
    private bool isVisible { get; set; }

    private APIUser user = APIUser.DummyUser(-1);

    private ClickableContainer content;

    private DrawableBanner banner;
    private DrawableAvatar avatar;
    private AvatarEdit avatarEdit;
    private BannerEdit bannerEdit;
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
                                    bannerEdit = new BannerEdit
                                    {
                                        Overlay = this,
                                        User = user
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
                                                    },
                                                    avatarEdit = new AvatarEdit
                                                    {
                                                        Overlay = this,
                                                        User = user
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
            avatarEdit.User = user;
            bannerEdit.User = user;

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
        isVisible = false;
    }

    public override void Show()
    {
        this.FadeIn(200);
        content.ScaleTo(1, 400, Easing.OutQuint);
        isVisible = true;
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

    public bool OnPressed(KeyBindingPressEvent<FluXisKeybind> e)
    {
        switch (e.Action)
        {
            case FluXisKeybind.Back:
                Hide();
                break;
        }

        return true;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisKeybind> e) { }

    private partial class AvatarEdit : Container
    {
        public ProfileOverlay Overlay;
        public APIUser User;

        [Resolved]
        private FluXisScreenStack stack { get; set; }

        [Resolved]
        private FluXisConfig config { get; set; }

        [Resolved]
        private NotificationOverlay notifications { get; set; }

        [Resolved]
        private Fluxel fluxel { get; set; }

        private bool canEdit => fluxel.LoggedInUser.ID == User.ID;

        private Container content;
        private SpriteIcon icon;

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.Both;

            InternalChild = content = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Alpha = 0,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Colour4.Black,
                        Alpha = 0.5f
                    },
                    icon = new SpriteIcon
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Icon = FontAwesome.Solid.PencilAlt,
                        Size = new Vector2(50)
                    }
                }
            };
        }

        protected override bool OnClick(ClickEvent e)
        {
            if (!canEdit) return false;

            Overlay.Hide();
            stack.Push(new FileImportScreen
            {
                OnFileSelected = file =>
                {
                    var notif = new LoadingNotification
                    {
                        TextLoading = "Uploading avatar...",
                        TextSuccess = "Avatar uploaded! Restart the game to see the changes.",
                        TextFailure = "Failed to upload avatar!"
                    };

                    notifications.AddNotification(notif);

                    byte[] data = File.ReadAllBytes(file.FullName);

                    var request = new WebRequest($"{fluxel.Endpoint.APIUrl}/account/update/avatar");
                    request.AllowInsecureRequests = true;
                    request.AddHeader("Authorization", $"{config.Get<string>(FluXisSetting.Token)}");
                    request.AddFile("avatar", data);
                    request.Method = HttpMethod.Post;

                    try
                    {
                        request.Perform();
                        var resp = JsonConvert.DeserializeObject<APIResponse<dynamic>>(request.GetResponseString() ?? string.Empty);

                        if (resp.Status == 200)
                            notif.State = LoadingState.Loaded;
                        else
                        {
                            Logger.Log($"Failed to upload avatar: {resp.Message}", LoggingTarget.Runtime, LogLevel.Error);
                            notif.TextFailure = $"Failed to upload avatar: {resp.Message}";
                            notif.State = LoadingState.Failed;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"Failed to upload avatar: {ex.Message}", LoggingTarget.Runtime, LogLevel.Error);
                        notif.State = LoadingState.Failed;
                    }
                },
                AllowedExtensions = FluXisGame.IMAGE_EXTENSIONS
            });

            return true;
        }

        protected override bool OnMouseDown(MouseDownEvent e)
        {
            icon.ScaleTo(0.9f, 3000, Easing.OutQuint);
            return true;
        }

        protected override void OnMouseUp(MouseUpEvent e)
        {
            icon.ScaleTo(1, 600, Easing.OutElastic);
        }

        protected override bool OnHover(HoverEvent e)
        {
            if (!canEdit) return false;

            content.FadeIn(200);
            return true;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            content.FadeOut(200);
            icon.ScaleTo(1, 400, Easing.OutQuint);
        }
    }

    private partial class BannerEdit : Container
    {
        public ProfileOverlay Overlay;
        public APIUser User;

        [Resolved]
        private FluXisScreenStack stack { get; set; }

        [Resolved]
        private FluXisConfig config { get; set; }

        [Resolved]
        private NotificationOverlay notifications { get; set; }

        [Resolved]
        private Fluxel fluxel { get; set; }

        private bool canEdit => fluxel.LoggedInUser.ID == User.ID;

        private Container content;
        private SpriteIcon icon;

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.Both;

            InternalChild = content = new Container
            {
                AutoSizeAxes = Axes.Both,
                Anchor = Anchor.TopRight,
                Origin = Anchor.TopRight,
                Margin = new MarginPadding { Horizontal = 15, Vertical = 10 },
                Alpha = 0,
                Children = new Drawable[]
                {
                    icon = new SpriteIcon
                    {
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Icon = FontAwesome.Solid.PencilAlt,
                        Size = new Vector2(15)
                    },
                    new FluXisSpriteText
                    {
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Text = "Change Banner",
                        X = 20
                    }
                }
            };
        }

        protected override bool OnClick(ClickEvent e)
        {
            if (!canEdit) return false;

            Overlay.Hide();
            stack.Push(new FileImportScreen
            {
                OnFileSelected = file =>
                {
                    var notif = new LoadingNotification
                    {
                        TextLoading = "Uploading banner...",
                        TextSuccess = "Banner uploaded! QuickRestart the game to see the changes.",
                        TextFailure = "Failed to upload banner!"
                    };

                    notifications.AddNotification(notif);

                    byte[] data = File.ReadAllBytes(file.FullName);

                    var request = new WebRequest($"{fluxel.Endpoint.APIUrl}/account/update/banner");
                    request.AllowInsecureRequests = true;
                    request.AddHeader("Authorization", $"{config.Get<string>(FluXisSetting.Token)}");
                    request.AddFile("banner", data);
                    request.Method = HttpMethod.Post;

                    try
                    {
                        request.Perform();
                        var resp = JsonConvert.DeserializeObject<APIResponse<dynamic>>(request.GetResponseString() ?? string.Empty);

                        if (resp.Status == 200)
                            notif.State = LoadingState.Loaded;
                        else
                        {
                            Logger.Log($"Failed to upload banner: {resp.Message}", LoggingTarget.Runtime, LogLevel.Error);
                            notif.TextFailure = $"Failed to upload banner: {resp.Message}";
                            notif.State = LoadingState.Failed;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log($"Failed to upload banner: {ex.Message}", LoggingTarget.Runtime, LogLevel.Error);
                        notif.State = LoadingState.Failed;
                    }
                },
                AllowedExtensions = FluXisGame.IMAGE_EXTENSIONS
            });

            return true;
        }

        protected override bool OnHover(HoverEvent e)
        {
            if (!canEdit) return false;

            content.FadeIn(200);
            return true;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            content.FadeOut(200);
            icon.ScaleTo(1, 400, Easing.OutQuint);
        }
    }
}

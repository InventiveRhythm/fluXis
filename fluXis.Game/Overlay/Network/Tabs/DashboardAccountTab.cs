using fluXis.Game.Graphics.Drawables;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface;
using fluXis.Game.Graphics.UserInterface.Files;
using fluXis.Game.Online;
using fluXis.Game.Online.API.Models.Users;
using fluXis.Game.Online.API.Requests.Account;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Overlay.Network.Tabs.Account;
using fluXis.Game.Overlay.Notifications;
using fluXis.Game.Overlay.Notifications.Tasks;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Logging;
using osuTK;

namespace fluXis.Game.Overlay.Network.Tabs;

public partial class DashboardAccountTab : DashboardTab
{
    public override string Title => "Account";
    public override IconUsage Icon => FontAwesome6.Solid.Gear;

    [Resolved]
    private FluxelClient fluxel { get; set; }

    [Resolved]
    private NotificationManager notifications { get; set; }

    [Resolved]
    private FluXisGameBase game { get; set; }

    private APIEditingUser user;
    private Container editContent;
    private LoadingIcon loadingIcon;

    private DrawableAvatar avatar;
    private DrawableBanner banner;

    private IdleTracker socialsTracker;
    private DashboardAccountCategory socialsCategory;
    private DashboardAccountEntry twitterEntry;
    private DashboardAccountEntry youtubeEntry;
    private DashboardAccountEntry twitchEntry;
    private DashboardAccountEntry discordEntry;

    private IdleTracker displayNameTracker;
    private DashboardAccountEntry displayNameEntry;
    private IdleTracker aboutmeTracker;
    private DashboardAccountEntry aboutmeEntry;

    [BackgroundDependencyLoader]
    private void load()
    {
        Content.Children = new Drawable[]
        {
            editContent = new Container
            {
                RelativeSizeAxes = Axes.Both
            },
            loadingIcon = new LoadingIcon
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Alpha = 0
            },
            socialsTracker = new IdleTracker(3000, socialsTrigger, () => socialsCategory.LoadingIcon.FadeIn(400)),
            displayNameTracker = new IdleTracker(3000, displayNameTrigger, () => displayNameEntry.LoadingIcon.FadeIn(400)),
            aboutmeTracker = new IdleTracker(3000, aboutmeTrigger, () => aboutmeEntry.LoadingIcon.FadeIn(400))
        };
    }

    private FillFlowContainer createContent()
    {
        return new FillFlowContainer
        {
            Width = 1200,
            AutoSizeAxes = Axes.Y,
            Direction = FillDirection.Full,
            Anchor = Anchor.TopCentre,
            Origin = Anchor.TopCentre,
            Padding = new MarginPadding { Horizontal = 50, Vertical = 20 },
            Spacing = new Vector2(50),
            Children = new Drawable[]
            {
                new Container
                {
                    Width = 1100,
                    Height = 250,
                    Children = new Drawable[]
                    {
                        new ClickableContainer
                        {
                            Size = new Vector2(250),
                            CornerRadius = 30,
                            Masking = true,
                            Child = avatar = new DrawableAvatar(user)
                            {
                                RelativeSizeAxes = Axes.Both,
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                FillMode = FillMode.Fill
                            },
                            Action = () =>
                            {
                                game.Overlay = new FileSelect
                                {
                                    OnFileSelected = file =>
                                    {
                                        var notif = new TaskNotificationData()
                                        {
                                            Text = "Avatar Update",
                                            TextWorking = "Uploading..."
                                        };

                                        notifications.AddTask(notif);

                                        var req = new AvatarUploadRequest(file);
                                        req.Progress += (cur, max) => notif.Progress = cur / (float)max;
                                        req.Success += res =>
                                        {
                                            notif.State = res.Status == 200 ? LoadingState.Complete : LoadingState.Failed;
                                            UserCache.TriggerAvatarUpdate(user.ID);
                                        };
                                        req.Failure += ex =>
                                        {
                                            notif.State = LoadingState.Failed;
                                            Logger.Error(ex, "Failed to upload avatar!");
                                        };
                                        req.PerformAsync(fluxel);
                                    },
                                    AllowedExtensions = FluXisGame.IMAGE_EXTENSIONS
                                };
                            }
                        },
                        new ClickableContainer
                        {
                            Size = new Vector2(750, 250),
                            CornerRadius = 30,
                            Masking = true,
                            Anchor = Anchor.TopRight,
                            Origin = Anchor.TopRight,
                            Child = banner = new DrawableBanner(user)
                            {
                                RelativeSizeAxes = Axes.Both,
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                FillMode = FillMode.Fill
                            },
                            Action = () =>
                            {
                                game.Overlay = new FileSelect
                                {
                                    OnFileSelected = file =>
                                    {
                                        var notif = new TaskNotificationData()
                                        {
                                            Text = "Banner Update",
                                            TextWorking = "Uploading..."
                                        };

                                        notifications.AddTask(notif);

                                        var req = new BannerUploadRequest(file);
                                        req.Progress += (cur, max) => notif.Progress = cur / (float)max;
                                        req.Success += res =>
                                        {
                                            notif.State = res.Status == 200 ? LoadingState.Complete : LoadingState.Failed;
                                            UserCache.TriggerBannerUpdate(user.ID);
                                        };
                                        req.Failure += ex =>
                                        {
                                            notif.State = LoadingState.Failed;
                                            Logger.Error(ex, "Failed to upload banner!");
                                        };
                                        req.PerformAsync(fluxel);
                                    },
                                    AllowedExtensions = FluXisGame.IMAGE_EXTENSIONS
                                };
                            }
                        }
                    }
                },
                new DashboardAccountCategory("Account")
                {
                    Children = new Drawable[]
                    {
                        new DashboardAccountEntry
                        {
                            Title = "Username",
                            Default = user.Username,
                            ReadOnly = true
                        },
                        new DashboardAccountEntry
                        {
                            Title = "E-Mail",
                            Default = StringUtils.CensorEmail(user.Email),
                            ReadOnly = true
                        }
                    }
                },
                new DashboardAccountCategory("Password")
                {
                    Children = new Drawable[]
                    {
                        new DashboardAccountEntry
                        {
                            Title = "New Password",
                            Default = "",
                            Placeholder = "...",
                            ReadOnly = true
                        },
                        new DashboardAccountEntry
                        {
                            Title = "Confirm Password",
                            Default = "",
                            Placeholder = "...",
                            ReadOnly = true
                        }
                    }
                },
                socialsCategory = new DashboardAccountCategory("Socials")
                {
                    Children = new Drawable[]
                    {
                        twitterEntry = new DashboardAccountEntry
                        {
                            Title = "Twitter",
                            Default = user.Socials.Twitter,
                            OnChange = () => socialsTracker.Reset()
                        },
                        youtubeEntry = new DashboardAccountEntry
                        {
                            Title = "YouTube",
                            Default = user.Socials.YouTube,
                            OnChange = () => socialsTracker.Reset()
                        },
                        twitchEntry = new DashboardAccountEntry
                        {
                            Title = "Twitch",
                            Default = user.Socials.Twitch,
                            OnChange = () => socialsTracker.Reset()
                        },
                        discordEntry = new DashboardAccountEntry
                        {
                            Title = "Discord",
                            Default = user.Socials.Discord,
                            OnChange = () => socialsTracker.Reset()
                        }
                    }
                },
                new DashboardAccountCategory("Vanity")
                {
                    Children = new Drawable[]
                    {
                        displayNameEntry = new DashboardAccountEntry
                        {
                            Title = "Display Name",
                            Placeholder = "...",
                            Default = user.DisplayName,
                            OnChange = () => displayNameTracker.Reset()
                        },
                        aboutmeEntry = new DashboardAccountEntry
                        {
                            Title = "About Me",
                            Placeholder = "...",
                            Default = user.AboutMe,
                            OnChange = () => aboutmeTracker.Reset()
                        }
                    }
                }
            }
        };
    }

    private async void socialsTrigger()
    {
        var req = new SocialUpdateRequest(twitterEntry.Value, youtubeEntry.Value, twitchEntry.Value, discordEntry.Value);
        await req.PerformAsync(fluxel);

        if (req.Response.Status == 200)
        {
            socialsCategory.CompletedIcon.FadeIn(400).Delay(1000).FadeOut(400);
            socialsCategory.LoadingIcon.FadeOut(400);
        }
        else
            notifications.SendError("Failed to update socials!", req.Response.Message);
    }

    private async void displayNameTrigger()
    {
        var req = new DisplayNameUpdateRequest(displayNameEntry.Value);
        await req.PerformAsync(fluxel);

        if (req.Response.Status == 200)
        {
            displayNameEntry.CompletedIcon.FadeIn(400).Delay(1000).FadeOut(400);
            displayNameEntry.LoadingIcon.FadeOut(400);
        }
        else
            notifications.SendError("Failed to update display name!", req.Response.Message);
    }

    private async void aboutmeTrigger()
    {
        var req = new AboutMeUpdateRequest(aboutmeEntry.Value);
        await req.PerformAsync(fluxel);

        if (req.Response.Status == 200)
        {
            aboutmeEntry.CompletedIcon.FadeIn(400).Delay(1000).FadeOut(400);
            aboutmeEntry.LoadingIcon.FadeOut(400);
        }
        else
            notifications.SendError("Failed to update about me!", req.Response.Message);
    }

    public override void Enter()
    {
        base.Enter();

        editContent.Clear();
        loadingIcon.FadeIn(400);

        var req = new AccountSelfRequest();
        req.Success += res =>
        {
            if (res.Status != 200)
            {
                notifications.SendError("Failed to get self!", res.Message);
                return;
            }

            user = res.Data;
            editContent.Child = createContent();
            loadingIcon.FadeOut(400);
        };
        req.Failure += ex =>
        {
            loadingIcon.FadeOut(400);
            Logger.Error(ex, "Failed to get user!");
            notifications.SendError("Failed to get self!", ex.Message);
        };

        req.PerformAsync(fluxel);
    }
}

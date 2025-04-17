using System;
using System.IO;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface;
using fluXis.Graphics.UserInterface.Buttons;
using fluXis.Graphics.UserInterface.Buttons.Presets;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Files;
using fluXis.Graphics.UserInterface.Panel;
using fluXis.Online;
using fluXis.Online.API.Models.Users;
using fluXis.Online.API.Payloads.Users;
using fluXis.Online.API.Requests.Users;
using fluXis.Online.Drawables;
using fluXis.Online.Fluxel;
using fluXis.Overlay.Network.Tabs.Account;
using fluXis.Overlay.Notifications;
using fluXis.Overlay.Notifications.Tasks;
using fluXis.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Logging;
using osuTK;

namespace fluXis.Overlay.Network.Tabs;

#nullable enable

public partial class DashboardAccountTab : DashboardTab
{
    public override string Title => "Account";
    public override IconUsage Icon => FontAwesome6.Solid.Gear;
    public override DashboardTabType Type => DashboardTabType.Account;

    [Resolved]
    private IAPIClient api { get; set; } = null!;

    [Resolved]
    private UserCache users { get; set; } = null!;

    [Resolved]
    private NotificationManager notifications { get; set; } = null!;

    [Resolved]
    private PanelContainer panels { get; set; } = null!;

    private APIUser user = null!;
    private Container editContent = null!;
    private Container unsavedContent = null!;
    private LoadingIcon loadingIcon = null!;

    private DashboardAccountTextBox twitterEntry = null!;
    private DashboardAccountTextBox youtubeEntry = null!;
    private DashboardAccountTextBox twitchEntry = null!;
    private DashboardAccountTextBox discordEntry = null!;

    private DashboardAccountTextBox displayNameEntry = null!;
    private DashboardAccountTextBox aboutmeEntry = null!;
    private DashboardAccountTextBox pronounsEntry = null!;

    private bool hasUnsavedChanges;
    private bool saving;

    [BackgroundDependencyLoader]
    private void load()
    {
        Content.Children = new Drawable[]
        {
            editContent = new Container
            {
                RelativeSizeAxes = Axes.Both
            },
            unsavedContent = new CircularContainer
            {
                Width = 644,
                AutoSizeAxes = Axes.Y,
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                Margin = new MarginPadding(12),
                Masking = true,
                Alpha = 0,
                Y = 40,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = FluXisColors.Background3
                    },
                    new FluXisSpriteText
                    {
                        Text = "There are unsaved changes.",
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        WebFontSize = 14,
                        X = 16
                    },
                    new FillFlowContainer
                    {
                        AutoSizeAxes = Axes.Both,
                        Direction = FillDirection.Horizontal,
                        Anchor = Anchor.CentreRight,
                        Origin = Anchor.CentreRight,
                        Spacing = new Vector2(8),
                        Padding = new MarginPadding(8),
                        Children = new Drawable[]
                        {
                            new FluXisButton
                            {
                                Size = new Vector2(128, 32),
                                FontSize = FluXisSpriteText.GetWebFontSize(14),
                                Data = new CancelButtonData("Reset", reset)
                            },
                            new FluXisButton
                            {
                                Size = new Vector2(128, 32),
                                FontSize = FluXisSpriteText.GetWebFontSize(14),
                                Data = new ButtonData
                                {
                                    Text = "Save",
                                    Action = save,
                                    Color = FluXisColors.Highlight,
                                    TextColor = FluXisColors.Background2
                                }
                            }
                        }
                    }
                }
            },
            loadingIcon = new LoadingIcon
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Alpha = 0
            }
        };
    }

    private FillFlowContainer createContent()
    {
        if (user.Socials is null)
            return new FillFlowContainer();

        return new FillFlowContainer
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Direction = FillDirection.Vertical,
            Padding = new MarginPadding { Top = 12 },
            Spacing = new Vector2(16),
            Children = new Drawable[]
            {
                new Container
                {
                    RelativeSizeAxes = Axes.X,
                    Height = 250,
                    Children = new Drawable[]
                    {
                        new ClickableContainer
                        {
                            Size = new Vector2(250),
                            CornerRadius = 30,
                            Masking = true,
                            Child = new LoadWrapper<DrawableAvatar>
                            {
                                RelativeSizeAxes = Axes.Both,
                                LoadContent = () => new DrawableAvatar(user)
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    FillMode = FillMode.Fill
                                }
                            },
                            Action = () =>
                            {
                                panels.Content = new FileSelect
                                {
                                    OnFileSelected = file => uploadImage(file, false),
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
                            Child = new LoadWrapper<DrawableBanner>
                            {
                                RelativeSizeAxes = Axes.Both,
                                LoadContent = () => new DrawableBanner(user)
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    FillMode = FillMode.Fill
                                }
                            },
                            Action = () => panels.Content = new FileSelect
                            {
                                OnFileSelected = file => uploadImage(file, true),
                                AllowedExtensions = FluXisGame.IMAGE_EXTENSIONS
                            }
                        }
                    }
                },
                new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Direction = FillDirection.Full,
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Padding = new MarginPadding { Vertical = 20 },
                    Spacing = new Vector2(32),
                    Children = new Drawable[]
                    {
                        new DashboardAccountCategory("Account")
                        {
                            Children = new Drawable[]
                            {
                                new DashboardAccountTextBox
                                {
                                    Title = "Username",
                                    Default = user.Username,
                                    ReadOnly = true
                                },
                                new DashboardAccountTextBox
                                {
                                    Title = "E-Mail",
                                    Default = StringUtils.CensorEmail(user.Email),
                                    ReadOnly = true
                                }
                            }
                        },
                        new DashboardAccountCategory("Auth")
                        {
                            Children = new Drawable[]
                            {
                                new DashboardAccountButton
                                {
                                    LabelText = "Change Password",
                                    ButtonText = "Change",
                                    Alpha = .5f
                                },
                                new DashboardAccountButton
                                {
                                    LabelText = "Setup 2FA",
                                    ButtonText = "Setup",
                                    Alpha = .5f
                                }
                            }
                        },
                        new DashboardAccountCategory("Socials")
                        {
                            Children = new Drawable[]
                            {
                                twitterEntry = new DashboardAccountTextBox
                                {
                                    Title = "Twitter",
                                    Default = user.Socials.Twitter,
                                    OnChange = updateUnsavedStatus
                                },
                                youtubeEntry = new DashboardAccountTextBox
                                {
                                    Title = "YouTube",
                                    Default = user.Socials.YouTube,
                                    OnChange = updateUnsavedStatus
                                },
                                twitchEntry = new DashboardAccountTextBox
                                {
                                    Title = "Twitch",
                                    Default = user.Socials.Twitch,
                                    OnChange = updateUnsavedStatus
                                },
                                discordEntry = new DashboardAccountTextBox
                                {
                                    Title = "Discord",
                                    Default = user.Socials.Discord,
                                    OnChange = updateUnsavedStatus
                                }
                            }
                        },
                        new DashboardAccountCategory("Vanity")
                        {
                            Children = new Drawable[]
                            {
                                displayNameEntry = new DashboardAccountTextBox
                                {
                                    Title = "Display Name",
                                    Placeholder = "...",
                                    Default = user.DisplayName,
                                    OnChange = updateUnsavedStatus
                                },
                                aboutmeEntry = new DashboardAccountTextBox
                                {
                                    Title = "About Me",
                                    Placeholder = "...",
                                    Default = user.AboutMe,
                                    OnChange = updateUnsavedStatus
                                },
                                pronounsEntry = new DashboardAccountTextBox
                                {
                                    Title = "Pronouns",
                                    Placeholder = "../..",
                                    Default = user.Pronouns,
                                    OnChange = updateUnsavedStatus
                                }
                            }
                        }
                    }
                }
            }
        };
    }

    private void updateUnsavedStatus()
    {
        hasUnsavedChanges = false;

        hasUnsavedChanges |= twitterEntry.Value != user.Socials?.Twitter;
        hasUnsavedChanges |= youtubeEntry.Value != user.Socials?.YouTube;
        hasUnsavedChanges |= twitchEntry.Value != user.Socials?.Twitch;
        hasUnsavedChanges |= discordEntry.Value != user.Socials?.Discord;

        hasUnsavedChanges |= displayNameEntry.Value != user.DisplayName;
        hasUnsavedChanges |= aboutmeEntry.Value != user.AboutMe;
        hasUnsavedChanges |= pronounsEntry.Value != user.Pronouns;

        if (hasUnsavedChanges)
            unsavedContent.FadeIn(400).MoveToY(0, 600, Easing.OutQuint);
        else
            unsavedContent.FadeOut(400).MoveToY(40, 600, Easing.OutQuint);
    }

    private void save()
    {
        if (saving || !hasUnsavedChanges)
            return;

        saving = true;

        var req = new UserProfileUpdateRequest(user.ID, new UserProfileUpdatePayload
        {
            Twitter = getValue(user.Socials?.Twitter, twitterEntry.Value),
            YouTube = getValue(user.Socials?.YouTube, youtubeEntry.Value),
            Twitch = getValue(user.Socials?.Twitch, twitchEntry.Value),
            Discord = getValue(user.Socials?.Discord, discordEntry.Value),
            DisplayName = getValue(user.DisplayName, displayNameEntry.Value),
            AboutMe = getValue(user.AboutMe, aboutmeEntry.Value),
            Pronouns = getValue(user.Pronouns, pronounsEntry.Value)
        });

        req.Success += res =>
        {
            user = res.Data;
            saving = false;
            updateUnsavedStatus();
        };

        req.Failure += ex =>
        {
            updateUnsavedStatus();
            saving = false;
            notifications.SendError("Failed to save changes!", ex.Message);
        };

        api.PerformRequestAsync(req);

        string? getValue(string? original, string? val) => val == original ? null : val;
    }

    private void uploadImage(FileInfo file, bool banner)
    {
        var notification = new TaskNotificationData
        {
            Text = $"{(banner ? "Banner" : "Avatar")} Update",
            TextWorking = "Uploading..."
        };

        notifications.AddTask(notification);

        var bytes = File.ReadAllBytes(file.FullName);
        var b64 = Convert.ToBase64String(bytes);

        var parameters = new UserProfileUpdatePayload();

        if (banner)
            parameters.Banner = b64;
        else
            parameters.Avatar = b64;

        var req = new UserProfileUpdateRequest(user.ID, parameters);
        req.Progress += (cur, max) => notification.Progress = cur / (float)max;

        req.Failure += ex =>
        {
            notification.TextFailed = ex.Message;
            notification.State = LoadingState.Failed;
        };

        req.Success += res =>
        {
            if (banner)
                users.TriggerBannerUpdate(user.ID, res.Data.BannerHash);
            else
                users.TriggerAvatarUpdate(user.ID, res.Data.AvatarHash);

            notification.State = LoadingState.Complete;
        };

        api.PerformRequestAsync(req);
    }

    private void reset()
    {
        twitterEntry.Value = user.Socials?.Twitter;
        youtubeEntry.Value = user.Socials?.YouTube;
        twitchEntry.Value = user.Socials?.Twitch;
        discordEntry.Value = user.Socials?.Discord;

        displayNameEntry.Value = user.DisplayName;
        aboutmeEntry.Value = user.AboutMe;
        pronounsEntry.Value = user.Pronouns;

        updateUnsavedStatus();
    }

    public override void Enter()
    {
        base.Enter();

        editContent.Clear();
        loadingIcon.FadeIn(400);

        if (api.User.Value is null)
        {
            loadingIcon.FadeOut(400);
            return;
        }

        var req = new UserRequest(api.User.Value.ID);
        req.Success += res =>
        {
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

        api.PerformRequestAsync(req);
    }
}

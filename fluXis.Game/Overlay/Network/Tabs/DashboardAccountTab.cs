using System.Collections.Generic;
using System.Net.Http;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Buttons;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Overlay.Network.Tabs.Account;
using fluXis.Game.Overlay.Notification;
using Newtonsoft.Json;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Game.Overlay.Network.Tabs;

public partial class DashboardAccountTab : DashboardTab
{
    public override string Title => "Account";
    public override IconUsage Icon => FontAwesome.Solid.Cog;

    [Resolved]
    private Fluxel fluxel { get; set; }

    [Resolved]
    private NotificationOverlay notifications { get; set; }

    private string twitter;
    private string youtube;
    private string twitch;
    private string discord;

    [BackgroundDependencyLoader]
    private void load()
    {
        Content.Child = new FillFlowContainer
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Direction = FillDirection.Vertical,
            Spacing = new Vector2(0, 10),
            Children = new Drawable[]
            {
                new FluXisSpriteText
                {
                    FontSize = 24,
                    Text = "Profile"
                },
                new DashboardAccountText
                {
                    Title = "Username",
                    Value = fluxel.LoggedInUser?.Username
                },
                new DashboardAccountTextbox
                {
                    Title = "Display Name",
                    Value = fluxel.LoggedInUser?.DisplayName,
                    ShowSaveButton = true,
                    OnSave = val =>
                    {
                        var req = fluxel.CreateAPIRequest("/account/update/displayname", HttpMethod.Post);
                        req.AddRaw(val);
                        req.Perform();

                        var res = req.GetResponseString();
                        var json = JsonConvert.DeserializeObject<FluxelResponse<dynamic>>(res);

                        var notification = json.Status == 200 ? new SimpleNotification() : new ErrorNotification();
                        notification.Text = json.Message;
                        notifications.AddNotification(notification);
                    }
                },
                new FluXisSpriteText
                {
                    FontSize = 24,
                    Text = "Socials",
                    Margin = new MarginPadding { Top = 20 }
                },
                new DashboardAccountTextbox
                {
                    Title = "Twitter",
                    Value = twitter = fluxel.LoggedInUser?.Socials.Twitter,
                    OnTextChanged = val => twitter = val
                },
                new DashboardAccountTextbox
                {
                    Title = "YouTube",
                    Value = youtube = fluxel.LoggedInUser?.Socials.YouTube,
                    OnTextChanged = val => youtube = val
                },
                new DashboardAccountTextbox
                {
                    Title = "Twitch",
                    Value = twitch = fluxel.LoggedInUser?.Socials.Twitch,
                    OnTextChanged = val => twitch = val
                },
                new DashboardAccountTextbox
                {
                    Title = "Discord",
                    Value = discord = fluxel.LoggedInUser?.Socials.Discord,
                    OnTextChanged = val => discord = val
                },
                new FluXisButton
                {
                    Text = "Save",
                    Width = 100,
                    Origin = Anchor.TopRight,
                    Margin = new MarginPadding { Right = -450 }, // kinda hacky but it works
                    Height = 32,
                    Action = () =>
                    {
                        var req = fluxel.CreateAPIRequest("/account/update/socials", HttpMethod.Post);
                        req.AddRaw(JsonConvert.SerializeObject(new Dictionary<string, string>
                        {
                            { "twitter", twitter },
                            { "youtube", youtube },
                            { "twitch", twitch },
                            { "discord", discord }
                        }));
                        req.Perform();

                        var res = req.GetResponseString();
                        var json = JsonConvert.DeserializeObject<FluxelResponse<dynamic>>(res);

                        var notification = json.Status == 200 ? new SimpleNotification() : new ErrorNotification();
                        notification.Text = json.Message;
                        notifications.AddNotification(notification);
                    }
                }
            }
        };
    }
}

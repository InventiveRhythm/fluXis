using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Graphics.Drawables;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Menus;
using fluXis.Game.Graphics.UserInterface.Text;
using fluXis.Game.Online.API.Models.Chat;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Overlay.Notifications;
using fluXis.Game.Overlay.User;
using fluXis.Game.Utils;
using fluXis.Shared.API.Packets.Chat;
using fluXis.Shared.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.UserInterface;
using osuTK;

namespace fluXis.Game.Overlay.Chat;

public partial class DrawableChatMessage : Container
{
    [Resolved]
    private UserProfileOverlay profile { get; set; }

    [Resolved]
    private FluxelClient fluxel { get; set; }

    public ChatMessage InitialMessage { get; set; }
    public List<ChatMessage> Messages { get; } = new();

    private FillFlowContainer<MessageText> flow;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;

        ClickableContainer avatarContainer;

        var nameColor = FluXisColors.Text;

        if (InitialMessage.Sender.Groups.Any())
        {
            var group = InitialMessage.Sender.Groups.First();
            nameColor = Colour4.TryParseHex(group.Color, out var c) ? c : FluXisColors.Text;
        }

        InternalChildren = new Drawable[]
        {
            avatarContainer = new ClickableContainer
            {
                Size = new Vector2(40),
                Masking = true,
                CornerRadius = 5,
                Action = () => profile.ShowUser(InitialMessage.Sender.ID)
            },
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Direction = FillDirection.Vertical,
                Padding = new MarginPadding { Left = 50 },
                Children = new Drawable[]
                {
                    new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Direction = FillDirection.Horizontal,
                        Spacing = new Vector2(5),
                        Children = new Drawable[]
                        {
                            new FluXisSpriteText
                            {
                                Text = InitialMessage.Sender.Username,
                                FontSize = 22,
                                Colour = nameColor
                            },
                            new FluXisTooltipText
                            {
                                Text = getTime(),
                                TooltipText = getTooltip(),
                                FontSize = 18,
                                Colour = FluXisColors.Text2,
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft
                            }
                        }
                    },
                    flow = new FillFlowContainer<MessageText>
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Direction = FillDirection.Vertical,
                        Spacing = new Vector2(5)
                    }
                }
            }
        };

        LoadComponentAsync(new DrawableAvatar(InitialMessage.Sender)
        {
            RelativeSizeAxes = Axes.Both,
            ShowTooltip = true
        }, avatar =>
        {
            avatarContainer.Add(avatar);
            avatar.FadeInFromZero(200);
        });

        AddMessage(InitialMessage);
    }

    public void AddMessage(ChatMessage message)
    {
        Messages.Add(message);
        flow.Add(new MessageText
        {
            Message = message,
            Fluxel = fluxel
        });
    }

    public void RemoveMessage(string id)
    {
        flow.Remove(flow.FirstOrDefault(m => m.Message.ID == id), true);
        Messages.Remove(Messages.FirstOrDefault(m => m.ID == id));
    }

    private string getTime()
    {
        var date = TimeUtils.GetFromSeconds(InitialMessage.CreatedAtUnix);
        var today = DateTimeOffset.Now;

        if (date.Day == today.Day && date.Month == today.Month && date.Year == today.Year)
            return $"Today at {date.Hour:00}:{date.Minute:00}";
        if (date.Day == today.Day - 1 && date.Month == today.Month && date.Year == today.Year)
            return $"Yesterday at {date.Hour:00}:{date.Minute:00}";

        return $"{date.Hour:00}:{date.Minute:00} {StringUtils.NumberWithOrderSuffix(date.Day)} {date.GetMonth()[..3]}";
    }

    private string getTooltip()
    {
        var date = TimeUtils.GetFromSeconds(InitialMessage.CreatedAtUnix);
        return $"{date.GetWeekDay()}, {date.Day} {date.GetMonth()} {date.Year} at {date.Hour:00}:{date.Minute:00}";
    }

    private partial class MessageText : FluXisTextFlow, IHasContextMenu
    {
        public ChatMessage Message { get; init; }
        public FluxelClient Fluxel { get; init; }

        [Resolved]
        private NotificationManager notifications { get; set; }

        [BackgroundDependencyLoader]
        private void load()
        {
            FontSize = 18;
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;
            AddText(Message.Content);
        }

        public MenuItem[] ContextMenuItems
        {
            get
            {
                List<MenuItem> items = new List<MenuItem>
                {
                    Fluxel.LoggedInUser?.CanModerate() ?? false
                        ? new FluXisMenuItem("Delete", MenuItemType.Dangerous, delete)
                        : new FluXisMenuItem("Report", MenuItemType.Dangerous, report)
                };

                return items.ToArray();
            }
        }

        private async void delete()
        {
            await Fluxel.SendPacket(ChatDeletePacket.Create(Message.ID));
        }

        private /* async*/ void report()
        {
            notifications.SendText("This feature is not yet implemented.");
            // await Fluxel.SendPacket(new ChatMessageReportPacket { Id = Message.Id });
        }
    }
}

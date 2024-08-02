using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Graphics.Drawables;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Menus;
using fluXis.Game.Graphics.UserInterface.Text;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Overlay.User;
using fluXis.Game.Utils;
using fluXis.Shared.API.Packets.Chat;
using fluXis.Shared.Components.Chat;
using fluXis.Shared.Utils.Extensions;
using JetBrains.Annotations;
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
    private IAPIClient api { get; set; }

    public IChatMessage InitialMessage { get; set; }
    public List<IChatMessage> Messages { get; } = new();

    private FillFlowContainer<MessageText> flow;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;

        var nameColor = FluXisColors.Text;

        if (InitialMessage.Sender.Groups.Any())
        {
            var group = InitialMessage.Sender.Groups.First();
            nameColor = Colour4.TryParseHex(group.Color, out var c) ? c : FluXisColors.Text;
        }

        InternalChildren = new Drawable[]
        {
            new LoadWrapper<DrawableAvatar>
            {
                Size = new Vector2(48),
                Masking = true,
                CornerRadius = 8,
                LoadContent = () => new DrawableAvatar(InitialMessage.Sender)
                {
                    RelativeSizeAxes = Axes.Both,
                    ClickAction = () => profile.ShowUser(InitialMessage.Sender.ID),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre
                },
                OnComplete = a => a.FadeInFromZero(400)
            },
            new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Direction = FillDirection.Vertical,
                Padding = new MarginPadding { Left = 48 + 12 },
                Children = new Drawable[]
                {
                    new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Direction = FillDirection.Horizontal,
                        Spacing = new Vector2(4),
                        Children = new Drawable[]
                        {
                            new FluXisSpriteText
                            {
                                Text = InitialMessage.Sender.Username,
                                WebFontSize = 16,
                                Colour = nameColor,
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft
                            },
                            new FluXisTooltipText
                            {
                                Text = getTime(),
                                TooltipText = getTooltip(),
                                WebFontSize = 12,
                                Alpha = .8f,
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft
                            }
                        }
                    },
                    flow = new FillFlowContainer<MessageText>
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Direction = FillDirection.Vertical
                    }
                }
            }
        };

        AddMessage(InitialMessage);
    }

    public void AddMessage(IChatMessage message)
    {
        Messages.Add(message);
        flow.Add(new MessageText
        {
            Message = message,
            API = api
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
        public IChatMessage Message { get; init; }
        public IAPIClient API { get; init; }

        private const string link_regex = @"(http|https)://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?";

        [BackgroundDependencyLoader(true)]
        private void load([CanBeNull] FluXisGame game)
        {
            WebFontSize = 14;
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;

            var words = Message.Content.Split(' ');

            foreach (var word in words)
            {
                if (word.StartsWith('@')) // mention
                {
                    AddText(word, t => t.Colour = FluXisColors.Highlight);
                }
                else if (Regex.IsMatch(word, link_regex)) // link
                {
                    AddText<ClickableFluXisSpriteText>(word, t =>
                    {
                        t.Colour = FluXisColors.Link;
                        t.Action = () => game?.OpenLink(word);
                    });
                }
                else
                    AddText(word);

                AddText(" ");
            }
        }

        public MenuItem[] ContextMenuItems
        {
            get
            {
                var items = new List<MenuItem>();

                if (API.User.Value?.CanModerate() ?? false)
                    items.Add(new FluXisMenuItem("Delete", FontAwesome6.Solid.Trash, MenuItemType.Dangerous, delete));

                items.Add(new FluXisMenuItem("Report", FontAwesome6.Solid.Flag, MenuItemType.Dangerous, report) { Enabled = () => false });
                return items.ToArray();
            }
        }

        private async void delete()
        {
            await API.SendPacket(ChatDeletePacket.Create(Message.ID));
        }

        private void report()
        {
        }
    }
}

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
using fluXis.Game.Online.API.Models.Chat;
using fluXis.Game.Online.API.Requests.Chat;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Overlay.User;
using fluXis.Game.Utils;
using fluXis.Game.Utils.Extensions;
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
                        Children = new[]
                        {
                            createIcon(),
                            getName(InitialMessage.Sender.PreferredName).With(d => d.Anchor = d.Origin = Anchor.CentreLeft),
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

        if (!Messages.Any())
            Expire();
    }

    private Drawable createIcon()
    {
        var groups = InitialMessage.Sender.Groups;

        if (groups.Count == 0)
            return Empty().With(d => d.Alpha = 0);

        return new FluXisSpriteIcon
        {
            Size = new Vector2(16),
            Anchor = Anchor.CentreLeft,
            Origin = Anchor.CentreLeft,
            Colour = Colour4.FromHex(groups.First().Color),
            Icon = groups.First().ID switch
            {
                "fa" => FontAwesome6.Solid.Star,
                "purifier" => FontAwesome6.Solid.Diamond,
                "moderators" => FontAwesome6.Solid.ShieldHalved,
                "dev" => FontAwesome6.Solid.UserShield,
                "bot" => FontAwesome6.Solid.UserAstronaut,
                _ => FontAwesome6.Solid.User
            }
        };
    }

    private Drawable getName(string text)
    {
        if (InitialMessage.Sender.NamePaint is null)
        {
            return new FluXisTooltipText
            {
                Text = text,
                WebFontSize = 16
            };
        }

        return new GradientText
        {
            Text = text,
            WebFontSize = 16,
            Colour = InitialMessage.Sender.NamePaint.Colors.CreateColorInfo()
        };
    }

    private string getTime()
    {
        var date = TimeUtils.GetFromSeconds(InitialMessage.CreatedAtUnix);
        var today = DateTimeOffset.Now;

        if (date.Day == today.Day && date.Month == today.Month && date.Year == today.Year)
            return $"Today at {date.Hour:00}:{date.Minute:00}";
        if (date.Day == today.Day - 1 && date.Month == today.Month && date.Year == today.Year)
            return $"Yesterday at {date.Hour:00}:{date.Minute:00}";

        return $"{date.Hour:00}:{date.Minute:00} {date.Day.NumberWithOrderSuffix()} {date.GetMonth()[..3]}";
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
            => await API.PerformRequestAsync(new ChatMessageDeleteRequest(Message.Channel, Message.ID));

        private void report()
        {
        }
    }
}

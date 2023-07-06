using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Context;
using fluXis.Game.Graphics.Scroll;
using fluXis.Game.Online.Chat;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Online.Fluxel.Packets.Chat;
using fluXis.Game.Overlay.Notification;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;
using osuTK.Graphics;

namespace fluXis.Game.Overlay.Chat;

public partial class ChatOverlay : Container
{
    [Resolved]
    private Fluxel fluxel { get; set; }

    [Resolved]
    private NotificationOverlay notifications { get; set; }

    public string Channel { get; set; } = "general";

    private readonly Dictionary<string, List<ChatMessage>> messages = new();

    private FluXisTextBox textBox;
    private FillFlowContainer<DrawableChatMessage> flow;
    private FluXisScrollContainer scroll;

    private ClickableContainer content;
    private LoadingIcon loading;

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
                Action = Hide,
                Child = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color4.Black,
                    Alpha = 0.5f
                }
            },
            content = new ClickableContainer
            {
                RelativeSizeAxes = Axes.Both,
                RelativePositionAxes = Axes.Both,
                Y = 0.5f,
                Height = 0.4f,
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                Padding = new MarginPadding(20),
                Children = new Drawable[]
                {
                    new FluXisContextMenuContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        CornerRadius = 10,
                        Masking = true,
                        Children = new Drawable[]
                        {
                            new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Colour = FluXisColors.Background2
                            },
                            new Container
                            {
                                RelativeSizeAxes = Axes.Both,
                                Padding = new MarginPadding(20),
                                Children = new Drawable[]
                                {
                                    new Container
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Masking = true,
                                        Padding = new MarginPadding { Bottom = 40 },
                                        Child = scroll = new FluXisScrollContainer
                                        {
                                            ScrollbarAnchor = Anchor.TopRight,
                                            RelativeSizeAxes = Axes.Both,
                                            Child = flow = new FillFlowContainer<DrawableChatMessage>
                                            {
                                                RelativeSizeAxes = Axes.X,
                                                AutoSizeAxes = Axes.Y,
                                                Direction = FillDirection.Vertical,
                                                Spacing = new Vector2(0, 5)
                                            }
                                        }
                                    },
                                    textBox = new FluXisTextBox
                                    {
                                        Anchor = Anchor.BottomLeft,
                                        Origin = Anchor.BottomLeft,
                                        RelativeSizeAxes = Axes.X,
                                        Height = 30,
                                        PlaceholderText = "Type your message here..."
                                    }
                                }
                            }
                        }
                    },
                    loading = new LoadingIcon
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Size = new Vector2(30),
                        Alpha = 0
                    }
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        textBox.OnCommit += (sender, _) =>
        {
            fluxel.SendPacketAsync(new ChatMessagePacket
            {
                Channel = "general",
                Content = sender.Text
            });

            sender.Text = "";
        };

        fluxel.OnStatusChanged += status =>
        {
            Schedule(() =>
            {
                switch (status)
                {
                    case ConnectionStatus.Online:
                        fluxel.SendPacketAsync(new ChatHistoryPacket { Channel = Channel });
                        break;

                    case ConnectionStatus.Offline:
                    case ConnectionStatus.Connecting:
                    case ConnectionStatus.Reconnecting:
                    case ConnectionStatus.Failing:
                        flow.Clear();
                        messages.Clear();
                        loading.FadeIn(200);
                        break;
                }
            });
        };

        fluxel.RegisterListener<ChatMessage>(EventType.ChatMessage, response =>
        {
            var list = messages.GetValueOrDefault(response.Data.Channel, new List<ChatMessage>());
            list.Add(response.Data);
            messages[response.Data.Channel] = list;

            if (response.Data.Channel == Channel)
                addMessage(response.Data);
        });

        fluxel.RegisterListener<ChatMessage[]>(EventType.ChatHistory, response =>
        {
            response.Data = response.Data.OrderBy(x => x.Timestamp).ToArray();

            var first = response.Data.FirstOrDefault();

            if (first == null)
                return;

            var channel = first.Channel;

            var list = messages.GetValueOrDefault(channel, new List<ChatMessage>());
            list.AddRange(response.Data);
            messages[channel] = list;

            ScheduleAfterChildren(() => loading.FadeOut(200));

            if (channel != Channel) return;

            foreach (var message in response.Data) addMessage(message);
        });

        fluxel.RegisterListener<string>(EventType.ChatMessageDelete, res =>
        {
            if (res.Status != 200)
            {
                notifications.PostError(res.Message);
                return;
            }

            Schedule(() =>
            {
                var message = flow.FirstOrDefault(x => x.Messages.Any(m => m.Id == res.Data));
                message?.RemoveMessage(res.Data);
                if (message?.Messages.Count == 0)
                    flow.Remove(message, true);
            });
        });

        if (fluxel.Status == ConnectionStatus.Online)
            fluxel.SendPacketAsync(new ChatHistoryPacket { Channel = Channel });
    }

    private void addMessage(ChatMessage message)
    {
        Schedule(() =>
        {
            var last = flow.Count > 0 ? flow[^1] : null;

            if (last != null && last.InitialMessage.Sender.ID == message.Sender.ID)
                last.AddMessage(message);
            else
                flow.Add(new DrawableChatMessage { InitialMessage = message });

            ScheduleAfterChildren(() => scroll.ScrollToEnd());
        });
    }

    public override void Hide()
    {
        this.FadeOut(200);
        content.MoveToY(0.5f, 400, Easing.OutQuint);
    }

    public override void Show()
    {
        this.FadeIn(200);
        content.MoveToY(0, 400, Easing.OutQuint);
    }
}

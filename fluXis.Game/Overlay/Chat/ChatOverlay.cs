using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface;
using fluXis.Game.Graphics.UserInterface.Buttons;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Context;
using fluXis.Game.Graphics.UserInterface.Text;
using fluXis.Game.Input;
using fluXis.Game.Online.API.Models.Chat;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Overlay.Chat.UI;
using fluXis.Game.Overlay.Notifications;
using fluXis.Shared.API.Packets.Chat;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Graphics;

namespace fluXis.Game.Overlay.Chat;

public partial class ChatOverlay : OverlayContainer, IKeyBindingHandler<FluXisGlobalKeybind>
{
    [Resolved]
    private IAPIClient api { get; set; }

    [Resolved]
    private NotificationManager notifications { get; set; }

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
                    Alpha = .5f
                }
            },
            content = new ClickableContainer
            {
                RelativeSizeAxes = Axes.Both,
                Y = 50,
                Height = 0.4f,
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                Children = new Drawable[]
                {
                    new FluXisContextMenuContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        Children = new Drawable[]
                        {
                            new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Colour = FluXisColors.Background1
                            },
                            new Container
                            {
                                Width = 300,
                                RelativeSizeAxes = Axes.Y,
                                Children = new Drawable[]
                                {
                                    new Box
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Colour = FluXisColors.Background2
                                    },
                                    new FluXisScrollContainer
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Padding = new MarginPadding(10) { Bottom = 80 },
                                        Child = new FillFlowContainer
                                        {
                                            RelativeSizeAxes = Axes.X,
                                            AutoSizeAxes = Axes.Y,
                                            Direction = FillDirection.Vertical,
                                            Spacing = new Vector2(0, 10),
                                            Children = new Drawable[]
                                            {
                                                new FluXisSpriteText { Text = "Channels" },
                                                new ChatChannelButton { Channel = "general" }
                                            }
                                        }
                                    },
                                    new Container
                                    {
                                        RelativeSizeAxes = Axes.X,
                                        Height = 80,
                                        Anchor = Anchor.BottomLeft,
                                        Origin = Anchor.BottomLeft,
                                        Padding = new MarginPadding(10),
                                        Child = new FluXisButton
                                        {
                                            RelativeSizeAxes = Axes.Both,
                                            Text = "Add Channel"
                                        }
                                    }
                                }
                            },
                            new Container
                            {
                                RelativeSizeAxes = Axes.Both,
                                Padding = new MarginPadding(20) { Left = 320 },
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
                                                Spacing = new Vector2(0, 10)
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
        base.LoadComplete();

        textBox.OnCommit += (sender, _) =>
        {
            api.SendPacketAsync(ChatMessagePacket.CreateC2S(textBox.Text, Channel));
            sender.Text = "";
        };

        api.Status.BindValueChanged(updateStatus, true);

        api.RegisterListener<ChatMessagePacket>(EventType.ChatMessage, response =>
        {
            if (response.Data == null)
                return;

            var channel = response.Data.ChatMessage.Channel;
            var list = messages.GetValueOrDefault(channel, new List<ChatMessage>());

            var message = (ChatMessage)response.Data.ChatMessage;
            list.Add(message);
            messages[channel] = list;

            if (channel == Channel)
                addMessage(message);
        });

        api.RegisterListener<ChatHistoryPacket>(EventType.ChatHistory, response =>
        {
            var data = response.Data!.Messages.OrderBy(x => x.CreatedAtUnix).Cast<ChatMessage>().ToArray();
            var first = data.FirstOrDefault();

            if (first == null)
                return;

            var channel = first.Channel;

            var list = messages.GetValueOrDefault(channel, new List<ChatMessage>());
            list.AddRange(data);
            messages[channel] = list;

            ScheduleAfterChildren(() => loading.FadeOut(200));

            if (channel != Channel) return;

            foreach (var message in data) addMessage(message);
        });

        api.RegisterListener<ChatDeletePacket>(EventType.ChatMessageDelete, res =>
        {
            if (!res.Success)
            {
                notifications.SendError(res.Message);
                return;
            }

            Schedule(() =>
            {
                var message = flow.FirstOrDefault(x => x.Messages.Any(m => m.ID == res.Data!.MessageID));
                message?.RemoveMessage(res.Data!.MessageID);
                if (message?.Messages.Count == 0)
                    flow.Remove(message, true);
            });
        });

        if (api.Status.Value == ConnectionStatus.Online)
            api.SendPacketAsync(ChatHistoryPacket.CreateC2S(Channel));
    }

    private void updateStatus(ValueChangedEvent<ConnectionStatus> e)
    {
        Schedule(() =>
        {
            switch (e.NewValue)
            {
                case ConnectionStatus.Online:
                    api.SendPacketAsync(ChatHistoryPacket.CreateC2S(Channel));
                    break;

                default:
                    flow.Clear();
                    messages.Clear();
                    loading.FadeIn(200);
                    break;
            }
        });
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

    protected override void PopIn()
    {
        this.FadeIn(200);
        content.MoveToY(0, 400, Easing.OutQuint);
    }

    protected override void PopOut()
    {
        this.FadeOut(200);
        content.MoveToY(50, 400, Easing.OutQuint);
    }

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

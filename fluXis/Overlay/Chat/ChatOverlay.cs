using System.Linq;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface;
using fluXis.Graphics.UserInterface.Buttons;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Context;
using fluXis.Graphics.UserInterface.Panel;
using fluXis.Graphics.UserInterface.Text;
using fluXis.Input;
using fluXis.Online.API.Models.Chat;
using fluXis.Online.Chat;
using fluXis.Online.Fluxel;
using fluXis.Overlay.Chat.UI;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Graphics;

namespace fluXis.Overlay.Chat;

public partial class ChatOverlay : OverlayContainer, IKeyBindingHandler<FluXisGlobalKeybind>
{
    [Resolved]
    private IAPIClient api { get; set; }

    [Resolved]
    private ChatClient client { get; set; }

    [Resolved]
    private PanelContainer panels { get; set; }

    public Bindable<string> Channel { get; } = new("general");

    private FillFlowContainer<ChatChannelButton> channels;
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
            new FullInputBlockingContainer
            {
                RelativeSizeAxes = Axes.Both,
                OnClickAction = Hide,
                Child = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color4.Black,
                    Alpha = .5f
                }
            },
            content = new ClickableContainer
            {
                RelativeSizeAxes = Axes.X,
                Y = 50,
                Height = 530,
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
                                        Padding = new MarginPadding(20) { Bottom = 80 },
                                        Child = new FillFlowContainer
                                        {
                                            RelativeSizeAxes = Axes.X,
                                            AutoSizeAxes = Axes.Y,
                                            Direction = FillDirection.Vertical,
                                            Spacing = new Vector2(10),
                                            Children = new Drawable[]
                                            {
                                                new FluXisSpriteText { Text = "Channels" },
                                                channels = new FillFlowContainer<ChatChannelButton>
                                                {
                                                    RelativeSizeAxes = Axes.X,
                                                    AutoSizeAxes = Axes.Y,
                                                    Direction = FillDirection.Vertical,
                                                    Spacing = new Vector2(10),
                                                }
                                            }
                                        }
                                    },
                                    new Container
                                    {
                                        RelativeSizeAxes = Axes.X,
                                        Height = 80,
                                        Anchor = Anchor.BottomLeft,
                                        Origin = Anchor.BottomLeft,
                                        Padding = new MarginPadding(20),
                                        Child = new FluXisButton
                                        {
                                            RelativeSizeAxes = Axes.Both,
                                            Text = "Add Channel",
                                            Action = () => panels.Content = new ChatChannelsPanel { OnJoinAction = chan => Channel.Value = chan }
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
                                        Padding = new MarginPadding { Bottom = 60 },
                                        Child = scroll = new FluXisScrollContainer
                                        {
                                            ScrollbarAnchor = Anchor.TopRight,
                                            RelativeSizeAxes = Axes.Both,
                                            Child = flow = new FillFlowContainer<DrawableChatMessage>
                                            {
                                                RelativeSizeAxes = Axes.X,
                                                AutoSizeAxes = Axes.Y,
                                                Direction = FillDirection.Vertical,
                                                Spacing = new Vector2(0, 12)
                                            }
                                        }
                                    },
                                    textBox = new FluXisTextBox
                                    {
                                        PlaceholderText = "Type your message here...",
                                        RelativeSizeAxes = Axes.X,
                                        Height = 50,
                                        SidePadding = 15,
                                        CornerRadius = 8,
                                        FontSize = FluXisSpriteText.GetWebFontSize(16),
                                        Anchor = Anchor.BottomLeft,
                                        Origin = Anchor.BottomLeft
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
            client.GetChannel(Channel.Value)?.SendMessage(sender.Text);
            sender.Text = "";
        };

        api.Status.BindValueChanged(updateStatus, true);

        client.Channels.ForEach(addChannel);
        client.ChannelJoined += addChannel;
        client.ChannelParted += removeChannel;

        Channel.BindValueChanged(e =>
        {
            flow.Clear();
            var chan = client.GetChannel(e.NewValue);
            chan?.Messages.ForEach(addMessage);
        });

        /*api.RegisterListener<ChatDeletePacket>(EventType.ChatMessageDelete, res =>
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
        });*/
    }

    private void updateStatus(ValueChangedEvent<ConnectionStatus> e) => Schedule(() =>
    {
        switch (e.NewValue)
        {
            case ConnectionStatus.Online:
                loading.Hide();
                break;

            default:
                flow.Clear();
                loading.Show();
                break;
        }
    });

    private void addChannel(ChatChannel channel)
    {
        channels.Add(new ChatChannelButton(channel, Channel.GetBoundCopy()));

        channel.Messages.ForEach(addMessage);
        channel.OnMessage += addMessage;
        channel.OnMessageRemoved += removeMessage;
    }

    private void removeChannel(ChatChannel channel)
    {
        var button = channels.FirstOrDefault(x => x.Channel == channel);

        if (button is null)
            return;

        channel.OnMessage -= addMessage;
        channel.OnMessageRemoved -= removeMessage;
        channels.Remove(button, true);
    }

    private void addMessage(APIChatMessage message) => Schedule(() =>
    {
        if (message.Channel != Channel.Value)
            return;

        var atEnd = scroll.IsScrolledToEnd();
        var last = flow.Count > 0 ? flow[^1] : null;

        if (last != null && last.InitialMessage.Sender.ID == message.Sender.ID && message.CreatedAtUnix - last.Messages.Last().CreatedAtUnix < 60 * 5)
            last.AddMessage(message);
        else
            flow.Add(new DrawableChatMessage { InitialMessage = message });

        if (atEnd)
            ScheduleAfterChildren(() => scroll.ScrollToEnd());
    });

    private void removeMessage(APIChatMessage message) => Schedule(() =>
    {
        if (message.Channel != Channel.Value)
            return;

        var draw = flow.FirstOrDefault(d => d.Messages.Any(m => m.ID == message.ID));
        draw?.RemoveMessage(message.ID);
    });

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

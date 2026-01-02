using System;
using System.Linq;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Buttons;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Panel;
using fluXis.Graphics.UserInterface.Text;
using fluXis.Localization;
using fluXis.Online.API.Models.Chat;
using fluXis.Online.Chat;
using fluXis.Online.Fluxel;
using fluXis.Overlay.Network.Tabs.Chat;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.EnumExtensions;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Overlay.Network.Tabs;

public partial class DashboardChatTab : DashboardTab
{
    public override LocalisableString Title => LocalizationStrings.Dashboard.Chat;
    public override IconUsage Icon => FontAwesome6.Solid.Message;
    public override DashboardTabType Type => DashboardTabType.Chat;
    public override float DashboardWidth => 1600;

    [Resolved]
    private IAPIClient api { get; set; }

    [Resolved]
    private ChatClient client { get; set; }

    [Resolved]
    private PanelContainer panels { get; set; }

    public Bindable<string> Channel { get; } = new("general");

    private FillFlowContainer<ChatChannelSection> channels;

    private FluXisScrollContainer scroll;
    private FillFlowContainer<DrawableChatMessage> flow;
    private FluXisTextBox textBox;

    private LoadingIcon loading;

    [BackgroundDependencyLoader]
    private void load()
    {
        Content.Child = new GridContainer
        {
            RelativeSizeAxes = Axes.Both,
            ColumnDimensions = new Dimension[]
            {
                new(GridSizeMode.Absolute, 300),
                new(GridSizeMode.Absolute, 32 + 4),
                new()
            },
            Content = new[]
            {
                new Drawable[]
                {
                    new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Children = new Drawable[]
                        {
                            new FluXisScrollContainer
                            {
                                RelativeSizeAxes = Axes.Both,
                                ScrollbarAnchor = Anchor.TopRight,
                                Padding = new MarginPadding { Bottom = 50 + 16 },
                                HideScrollbarOnInactivity = true,
                                Child = channels = new FillFlowContainer<ChatChannelSection>
                                {
                                    RelativeSizeAxes = Axes.X,
                                    AutoSizeAxes = Axes.Y,
                                    Padding = new MarginPadding { Right = 16 },
                                    Spacing = new Vector2(8),
                                    ChildrenEnumerable = Enum.GetValues<APIChannelType>()
                                                             .GetValuesInOrder()
                                                             .Select(x => new ChatChannelSection(x, Channel.GetBoundCopy()))
                                },
                            },
                            new FluXisButton
                            {
                                Text = "Add Channel",
                                RelativeSizeAxes = Axes.X,
                                Height = 50,
                                Action = () => panels.Content = new ChatChannelsPanel { OnJoinAction = chan => Channel.Value = chan },
                                Anchor = Anchor.BottomLeft,
                                Origin = Anchor.BottomLeft
                            }
                        }
                    },
                    new Circle
                    {
                        Width = 4,
                        RelativeSizeAxes = Axes.Y,
                        Colour = Theme.Background3,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre
                    },
                    new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Children = new Drawable[]
                        {
                            scroll = new FluXisScrollContainer
                            {
                                RelativeSizeAxes = Axes.Both,
                                ScrollbarAnchor = Anchor.TopRight,
                                Padding = new MarginPadding { Bottom = 48 + 16 },
                                Child = flow = new FillFlowContainer<DrawableChatMessage>
                                {
                                    RelativeSizeAxes = Axes.X,
                                    AutoSizeAxes = Axes.Y,
                                    Direction = FillDirection.Vertical,
                                    Spacing = new Vector2(12)
                                }
                            },
                            textBox = new FluXisTextBox
                            {
                                BackgroundActive = Theme.Background3,
                                BackgroundInactive = Theme.Background3,
                                PlaceholderText = "Type your message here...",
                                RelativeSizeAxes = Axes.X,
                                Height = 48,
                                SidePadding = 14,
                                CornerRadius = 8,
                                FontSize = FluXisSpriteText.GetWebFontSize(16),
                                Anchor = Anchor.BottomLeft,
                                Origin = Anchor.BottomLeft
                            },
                            loading = new LoadingIcon
                            {
                                Size = new Vector2(24),
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Alpha = 0
                            }
                        }
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
    }

    public void WaitForChannel(string ch)
    {
        if (client.Channels.Any(x => x.Name == ch))
        {
            Channel.Value = ch;
            return;
        }

        ScheduleAfterChildren(() => WaitForChannel(ch));
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
        channels.First(x => x.Type == channel.Type).AddChannel(channel);

        channel.Messages.ForEach(addMessage);
        channel.OnMessage += addMessage;
        channel.OnMessageRemoved += removeMessage;
    }

    private void removeChannel(ChatChannel channel)
    {
        channels.First(x => x.Type == channel.Type).RemoveChannel(channel);

        channel.OnMessage -= addMessage;
        channel.OnMessageRemoved -= removeMessage;
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

    public override void Enter()
    {
        base.Enter();
        scroll.ScrollToEnd();
    }
}

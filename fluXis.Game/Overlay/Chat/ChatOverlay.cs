using System.Linq;
using fluXis.Game.Graphics;
using fluXis.Game.Online.Chat;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Online.Fluxel.Packets.Chat;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;
using osuTK.Graphics;

namespace fluXis.Game.Overlay.Chat;

public partial class ChatOverlay : Container
{
    private FluXisTextBox textBox;
    private FillFlowContainer<DrawableChatMessage> flow;
    private BasicScrollContainer scroll;

    private Container content;

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
            content = new Container
            {
                RelativeSizeAxes = Axes.Both,
                RelativePositionAxes = Axes.Both,
                Y = 0.5f,
                Height = 0.4f,
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                Padding = new MarginPadding(20),
                Child = new ClickableContainer
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
                                    Child = scroll = new BasicScrollContainer
                                    {
                                        ScrollbarVisible = false,
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
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        textBox.OnCommit += (sender, _) =>
        {
            Fluxel.SendPacket(new ChatMessagePacket
            {
                Channel = "general",
                Content = sender.Text
            });

            sender.Text = "";
        };

        Fluxel.RegisterListener<ChatMessage>(EventType.ChatMessage, response =>
        {
            Schedule(() =>
            {
                var last = flow.LastOrDefault();

                if (last != null && last.Message.Sender.Username == response.Data.Sender.Username)
                    last.AddMessage(response.Data);
                else
                    flow.Add(new DrawableChatMessage { Message = response.Data });

                ScheduleAfterChildren(() => scroll.ScrollToEnd());
            });
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

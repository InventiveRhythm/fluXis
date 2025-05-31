using fluXis.Audio;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Interaction;
using fluXis.Graphics.UserInterface.Menus;
using fluXis.Online.Chat;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace fluXis.Overlay.Chat.UI;

public partial class ChatChannelButton : Container, IHasContextMenu
{
    public ChatChannel Channel { get; }
    public IconUsage Icon { get; set; } = FontAwesome6.Solid.Hashtag;

    public MenuItem[] ContextMenuItems => new MenuItem[]
    {
        new FluXisMenuItem("Leave Channel", FontAwesome6.Solid.DoorOpen, MenuItemType.Dangerous, leave)
    };

    private Bindable<string> bind { get; }

    [Resolved]
    private UISamples samples { get; set; }

    [Resolved]
    private ChatClient client { get; set; }

    private HoverLayer hover;
    private FlashLayer flash;
    private Container content;

    public ChatChannelButton(ChatChannel channel, Bindable<string> bind)
    {
        Channel = channel;
        this.bind = bind;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 48;

        InternalChildren = new Drawable[]
        {
            content = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                CornerRadius = 8,
                Masking = true,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = FluXisColors.Background3
                    },
                    hover = new HoverLayer(),
                    flash = new FlashLayer(),
                    new FillFlowContainer
                    {
                        AutoSizeAxes = Axes.Both,
                        Direction = FillDirection.Horizontal,
                        Spacing = new Vector2(8),
                        Padding = new MarginPadding(14),
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Children = new Drawable[]
                        {
                            new FluXisSpriteIcon
                            {
                                Icon = Icon,
                                Size = new Vector2(20),
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft
                            },
                            new FluXisSpriteText
                            {
                                Text = Channel.Name,
                                WebFontSize = 14,
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft
                            }
                        }
                    }
                }
            }
        };
    }

    protected override bool OnClick(ClickEvent e)
    {
        flash.Show();
        samples.Click();
        bind.Value = Channel.Name;
        return true;
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        if (e.Button != MouseButton.Left)
            return false;

        content.ScaleTo(.95f, 1000, Easing.OutQuint);
        return true;
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        content.ScaleTo(1f, 800, Easing.OutElasticHalf);
    }

    protected override bool OnHover(HoverEvent e)
    {
        hover.Show();
        samples.Hover();
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        hover.Hide();
    }

    private void leave() => client.LeaveChannel(Channel.Name);
}

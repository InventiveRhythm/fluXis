using System.Collections.Generic;
using fluXis.Audio;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Interaction;
using fluXis.Graphics.UserInterface.Menus;
using fluXis.Graphics.UserInterface.Menus.Items;
using fluXis.Online.API.Models.Chat;
using fluXis.Online.Chat;
using fluXis.Online.Drawables.Images;
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

namespace fluXis.Overlay.Network.Tabs.Chat;

public partial class ChatChannelButton : Container, IHasContextMenu
{
    public ChatChannel Channel { get; }
    public IconUsage Icon { get; set; } = FontAwesome6.Solid.Hashtag;

    public MenuItem[] ContextMenuItems
    {
        get
        {
            var list = new List<MenuItem>();

            if (publicChannel)
                list.Add(new MenuActionItem("Leave Channel", FontAwesome6.Solid.DoorOpen, MenuItemType.Dangerous, leave));

            return list.ToArray();
        }
    }

    private Bindable<string> bind { get; }

    [Resolved]
    private UISamples samples { get; set; }

    [Resolved]
    private ChatClient client { get; set; }

    private HoverLayer hover;
    private FlashLayer flash;
    private Container content;

    private string channelName;
    private bool publicChannel => Channel.Type == APIChannelType.Public;

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

        channelName = Channel.Name;

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
                        Colour = Theme.Background3
                    },
                    hover = new HoverLayer(),
                    flash = new FlashLayer(),
                    new FillFlowContainer
                    {
                        AutoSizeAxes = Axes.Both,
                        Direction = FillDirection.Horizontal,
                        Spacing = new Vector2(8),
                        Padding = new MarginPadding(6),
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Children = new[]
                        {
                            createIcon().With(x => x.Anchor = x.Origin = Anchor.CentreLeft),
                            new FluXisSpriteText
                            {
                                Text = channelName,
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

    private Drawable createIcon()
    {
        switch (Channel.Type)
        {
            case APIChannelType.Private:
            {
                var other = Channel.APIChannel.OtherUser(client.Self.ID);
                if (other is null) return Empty();

                channelName = other.PreferredName;

                return new LoadWrapper<DrawableAvatar>
                {
                    Size = new Vector2(32),
                    CornerRadius = 4,
                    Masking = true,
                    LoadContent = () => new DrawableAvatar(other) { RelativeSizeAxes = Axes.Both },
                    OnComplete = d => d.FadeInFromZero(400)
                };
            }

            case APIChannelType.Club:
            {
                if (Channel.APIChannel.Club is null) return Empty();

                channelName = Channel.APIChannel.Club.Name;

                return new LoadWrapper<DrawableClubIcon>
                {
                    Size = new Vector2(32),
                    CornerRadius = 4,
                    Masking = true,
                    LoadContent = () => new DrawableClubIcon(Channel.APIChannel.Club) { RelativeSizeAxes = Axes.Both },
                    OnComplete = d => d.FadeInFromZero(400)
                };
            }
        }

        return new FluXisSpriteIcon
        {
            Icon = Icon,
            Size = new Vector2(20),
            Margin = new MarginPadding(6)
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

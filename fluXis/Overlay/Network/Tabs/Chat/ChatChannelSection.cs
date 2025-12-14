using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.Sprites.Text;
using fluXis.Online.API.Models.Chat;
using fluXis.Online.Chat;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Overlay.Network.Tabs.Chat;

public partial class ChatChannelSection : FillFlowContainer
{
    private readonly Bindable<string> bind;

    public APIChannelType Type { get; }

    private readonly List<ChatChannelButton> channels = new();

    public ChatChannelSection(APIChannelType type, Bindable<string> bind)
    {
        this.bind = bind;
        Type = type;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Direction = FillDirection.Vertical;
        Spacing = new Vector2(8);

        InternalChildren = new Drawable[]
        {
            new FluXisSpriteText
            {
                Text = Type.GetDescription(),
                WebFontSize = 12,
                Alpha = 0.8f
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        updateVisibility();
    }

    public void AddChannel(ChatChannel channel)
    {
        var ch = new ChatChannelButton(channel, bind);
        channels.Add(ch);
        AddInternal(ch);
        updateVisibility();
    }

    public void RemoveChannel(ChatChannel channel)
    {
        var button = channels.FirstOrDefault(x => x.Channel == channel);
        if (button == null) return;

        channels.Remove(button);
        Remove(button, true);
        updateVisibility();
    }

    private void updateVisibility() => Alpha = channels.Count != 0 ? 1f : 0f;
}

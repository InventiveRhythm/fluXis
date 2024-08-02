using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using fluXis.Game.Online.Fluxel;
using fluXis.Shared.API;
using fluXis.Shared.API.Packets.Chat;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;

namespace fluXis.Game.Online.Chat;

public partial class ChatClient : Component
{
    [Resolved]
    private IAPIClient api { get; set; }

    public event Action<ChatChannel> ChannelJoined;
    public event Action<ChatChannel> ChannelParted;

    public IReadOnlyList<ChatChannel> Channels => channels.Values.ToImmutableList();

    private Dictionary<string, ChatChannel> channels { get; } = new();

    [BackgroundDependencyLoader]
    private void load()
    {
        api.RegisterListener<ChatMessagePacket>(EventType.ChatMessage, onMessage);
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        api.Status.BindValueChanged(statusChanged, true);
    }

    public void JoinChannel(string channel) { }

    [CanBeNull]
    public ChatChannel GetChannel(string channel)
        => !channels.TryGetValue(channel, out var chan) ? null : chan;

    private void statusChanged(ValueChangedEvent<ConnectionStatus> e) => Schedule(() =>
    {
        switch (e.NewValue)
        {
            case ConnectionStatus.Online:
                fetchJoinedChannels();
                break;

            default:
                channels.Keys.ForEach(removeChannel);
                break;
        }
    });

    private void fetchJoinedChannels()
    {
        addChannel("general");
        addChannel("mapping");
        addChannel("off-topic");
    }

    private void addChannel(string name)
    {
        var chan = new ChatChannel(name, api);
        channels.Add(name, chan);
        ChannelJoined?.Invoke(chan);
    }

    private void removeChannel(string name)
    {
        if (!channels.Remove(name, out var channel))
            return;

        ChannelParted?.Invoke(channel);
    }

    private void onMessage(FluxelReply<ChatMessagePacket> packet)
    {
        if (packet.Data?.ChatMessage is null || !channels.TryGetValue(packet.Data.ChatMessage.Channel, out var channel))
            return;

        channel.AddMessage(packet.Data.ChatMessage);
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);
        channels.Clear();
    }
}

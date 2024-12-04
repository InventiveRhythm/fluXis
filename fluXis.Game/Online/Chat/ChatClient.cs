using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using fluXis.Game.Online.API.Packets.Chat;
using fluXis.Game.Online.API.Requests.Chat;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Utils.Extensions;
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
        api.RegisterListener<ChatChannelJoinPacket>(EventType.ChatJoin, pk => addChannel(pk.Data!.Channel));
        api.RegisterListener<ChatChannelLeavePacket>(EventType.ChatLeave, pk => removeChannel(pk.Data!.Channel));
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        api.Status.BindValueChanged(statusChanged, true);
    }

    public void JoinChannel(string channel)
    {
        var req = new ChatJoinChannelRequest(channel);
        api.PerformRequestAsync(req);
    }

    public void LeaveChannel(string channel)
    {
        var req = new ChatLeaveChannelRequest(channel);
        api.PerformRequestAsync(req);
    }

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
        var req = new ChatJoinedChannelsRequest();
        req.Success += res => res.Data.ForEach(c => addChannel(c.Name));
        api.PerformRequestAsync(req);
    }

    private void addChannel(string name) => Scheduler.ScheduleIfNeeded(() =>
    {
        var chan = new ChatChannel(name, api);
        channels.Add(name, chan);
        ChannelJoined?.Invoke(chan);
    });

    private void removeChannel(string name) => Scheduler.ScheduleIfNeeded(() =>
    {
        if (!channels.Remove(name, out var channel))
            return;

        ChannelParted?.Invoke(channel);
    });

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

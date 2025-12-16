using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using fluXis.Online.API.Models.Chat;
using fluXis.Online.API.Models.Users;
using fluXis.Online.API.Requests.Chat;
using fluXis.Online.Fluxel;
using fluXis.Overlay.Network;
using fluXis.Overlay.Network.Tabs;
using fluXis.Utils.Extensions;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;

namespace fluXis.Online.Chat;

public partial class ChatClient : Component
{
    [Resolved]
    private IAPIClient api { get; set; }

    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private Dashboard dashboard { get; set; }

    public event Action<ChatChannel> ChannelJoined;
    public event Action<ChatChannel> ChannelParted;

    public APIUser Self => api.User.Value;
    public IReadOnlyList<ChatChannel> Channels => channels.Values.ToImmutableList();

    private Dictionary<string, ChatChannel> channels { get; } = new();

    [BackgroundDependencyLoader]
    private void load()
    {
        api.ChatChannelAdded += addChannel;
        api.ChatChannelRemoved += removeChannel;
        api.ChatMessageReceived += onMessage;
        api.ChatMessageRemoved += onMessageDelete;
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
        => channels.GetValueOrDefault(channel);

    public async Task CreatePrivateChannel(long target)
    {
        var req = new ChatCreateChannelRequest(target);
        await api.PerformRequestAsync(req);

        if (!req.Response.Success)
            return;

        dashboard?.Show<DashboardChatTab>().WaitForChannel(req.Response.Data);
    }

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
        req.Success += res => res.Data.ForEach(addChannel);
        api.PerformRequestAsync(req);
    }

    private void addChannel(APIChatChannel channel) => Scheduler.ScheduleIfNeeded(() =>
    {
        if (channels.ContainsKey(channel.Name))
            return;

        var chan = new ChatChannel(channel, api);
        channels.Add(channel.Name, chan);
        ChannelJoined?.Invoke(chan);
    });

    private void removeChannel(string name) => Scheduler.ScheduleIfNeeded(() =>
    {
        if (!channels.Remove(name, out var channel))
            return;

        ChannelParted?.Invoke(channel);
    });

    private void onMessage(APIChatMessage message)
    {
        if (!channels.TryGetValue(message.Channel, out var channel))
            return;

        channel.AddMessage(message);
    }

    private void onMessageDelete(string channel, string id)
    {
        if (!channels.TryGetValue(channel, out var c))
            return;

        c.DeleteMessage(id);
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);
        channels.Clear();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Online.API.Models.Chat;
using fluXis.Online.API.Requests.Chat;
using fluXis.Online.Fluxel;

namespace fluXis.Online.Chat;

public class ChatChannel
{
    public string Name => APIChannel.Name;
    public APIChannelType Type => APIChannel.Type;

    public APIChatChannel APIChannel { get; }
    private IAPIClient api { get; }

    public event Action<APIChatMessage> OnMessage;
    public event Action<APIChatMessage> OnMessageRemoved;

    public IReadOnlyList<APIChatMessage> Messages => messages;

    private List<APIChatMessage> messages { get; } = new();

    public ChatChannel(APIChatChannel channel, IAPIClient api)
    {
        APIChannel = channel;

        this.api = api;

        loadMessages();
    }

    private void loadMessages()
    {
        var req = new ChatMessagesRequest(Name);
        req.Success += res =>
        {
            res.Data.Sort((a, b) => a.CreatedAtUnix.CompareTo(b.CreatedAtUnix));
            res.Data.ForEach(AddMessage);
        };
        api.PerformRequestAsync(req);
    }

    public void AddMessage(APIChatMessage message)
    {
        messages.Add(message);
        OnMessage?.Invoke(message);
    }

    public void DeleteMessage(string id)
    {
        var msg = messages.FirstOrDefault(m => m.ID == id);

        if (msg is null)
            return;

        messages.Remove(msg);
        OnMessageRemoved?.Invoke(msg);
    }

    public void SendMessage(string content)
    {
        var req = new ChatMessageRequest(Name, content);
        api.PerformRequestAsync(req);
    }

    public override string ToString() => $"ChatChannel({Name})";
}

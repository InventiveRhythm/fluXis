using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Online.API.Models.Chat;
using fluXis.Online.API.Requests.Chat;
using fluXis.Online.Fluxel;

namespace fluXis.Online.Chat;

public class ChatChannel
{
    public string Name { get; }
    private IAPIClient api { get; }

    public event Action<IChatMessage> OnMessage;
    public event Action<IChatMessage> OnMessageRemoved;

    public IReadOnlyList<IChatMessage> Messages => messages;

    private List<IChatMessage> messages { get; } = new();

    public ChatChannel(string name, IAPIClient api)
    {
        Name = name;
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

    public void AddMessage(IChatMessage message)
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

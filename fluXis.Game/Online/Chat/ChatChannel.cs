using System;
using System.Collections.Generic;
using fluXis.Game.Online.API.Requests.Chat;
using fluXis.Game.Online.Fluxel;
using fluXis.Shared.Components.Chat;

namespace fluXis.Game.Online.Chat;

public class ChatChannel
{
    public string Name { get; }
    private IAPIClient api { get; }

    public event Action<IChatMessage> OnMessage;

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

    public void SendMessage(string content)
    {
        var req = new ChatMessageRequest(Name, content);
        api.PerformRequestAsync(req);
    }

    public override string ToString() => $"ChatChannel({Name})";
}

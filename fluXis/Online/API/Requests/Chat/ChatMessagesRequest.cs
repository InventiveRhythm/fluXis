using System.Collections.Generic;
using fluXis.Online.API.Models.Chat;

namespace fluXis.Online.API.Requests.Chat;

public class ChatMessagesRequest : APIRequest<List<APIChatMessage>>
{
    protected override string Path => $"/chat/channels/{channel}/messages";

    private string channel { get; }

    public ChatMessagesRequest(string channel)
    {
        this.channel = channel;
    }
}

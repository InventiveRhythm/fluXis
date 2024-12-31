using System.Net.Http;

namespace fluXis.Online.API.Requests.Chat;

public class ChatMessageDeleteRequest : APIRequest<dynamic>
{
    protected override string Path => $"/chat/channels/{channel}/messages/{id}";
    protected override HttpMethod Method => HttpMethod.Delete;

    private string channel { get; }
    private string id { get; }

    public ChatMessageDeleteRequest(string channel, string id)
    {
        this.channel = channel;
        this.id = id;
    }
}

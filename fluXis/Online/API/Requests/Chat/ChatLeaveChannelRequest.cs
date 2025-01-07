using System.Net.Http;

namespace fluXis.Online.API.Requests.Chat;

public class ChatLeaveChannelRequest : APIRequest<dynamic>
{
    protected override string Path => $"/chat/channels/{channel}/users/{APIClient.User.Value.ID}";
    protected override HttpMethod Method => HttpMethod.Delete;

    private string channel { get; }

    public ChatLeaveChannelRequest(string channel)
    {
        this.channel = channel;
    }
}

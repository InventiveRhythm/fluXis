using System.Net.Http;

namespace fluXis.Online.API.Requests.Chat;

public class ChatJoinChannelRequest : APIRequest<dynamic>
{
    protected override string Path => $"/chat/channels/{channel}/users/{APIClient.User.Value.ID}";
    protected override HttpMethod Method => HttpMethod.Put;

    private string channel { get; }

    public ChatJoinChannelRequest(string channel)
    {
        this.channel = channel;
    }
}

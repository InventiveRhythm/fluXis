using System.Net.Http;
using fluXis.Online.API.Payloads.Chat;
using fluXis.Utils;
using osu.Framework.IO.Network;

namespace fluXis.Online.API.Requests.Chat;

public class ChatCreateChannelRequest : APIRequest<string>
{
    protected override string Path => $"/chat/channels";
    protected override HttpMethod Method => HttpMethod.Post;

    private long target { get; }

    public ChatCreateChannelRequest(long target)
    {
        this.target = target;
    }

    protected override WebRequest CreateWebRequest(string url)
    {
        var req = base.CreateWebRequest(url);
        req.AddRaw(new ChatCreateChannelPayload(target).Serialize());
        return req;
    }
}

using System.Net.Http;
using fluXis.Shared.API.Parameters.Chat;
using fluXis.Shared.Components.Chat;
using fluXis.Shared.Utils;
using osu.Framework.IO.Network;

namespace fluXis.Game.Online.API.Requests.Chat;

public class ChatMessageRequest : APIRequest<IChatMessage>
{
    protected override string Path => $"/chat/channels/{channel}/messages";
    protected override HttpMethod Method => HttpMethod.Post;

    private string channel { get; }
    private string content { get; }

    public ChatMessageRequest(string channel, string content)
    {
        this.channel = channel;
        this.content = content;
    }

    protected override WebRequest CreateWebRequest(string url)
    {
        var req = base.CreateWebRequest(url);
        req.AddRaw(new ChatMessageParameters(content).Serialize());
        return req;
    }
}

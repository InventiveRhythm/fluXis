using System.Collections.Generic;
using fluXis.Shared.Components.Chat;

namespace fluXis.Game.Online.API.Requests.Chat;

public class ChatJoinedChannelsRequest : APIRequest<List<APIChatChannel>>
{
    protected override string Path => "/chat/channels/joined";
}

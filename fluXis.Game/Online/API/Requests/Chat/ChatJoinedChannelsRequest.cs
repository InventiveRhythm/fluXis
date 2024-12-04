using System.Collections.Generic;
using fluXis.Game.Online.API.Models.Chat;

namespace fluXis.Game.Online.API.Requests.Chat;

public class ChatJoinedChannelsRequest : APIRequest<List<APIChatChannel>>
{
    protected override string Path => "/chat/channels/joined";
}

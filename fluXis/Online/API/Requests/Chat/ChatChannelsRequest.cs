using System.Collections.Generic;
using fluXis.Online.API.Models.Chat;

namespace fluXis.Online.API.Requests.Chat;

public class ChatChannelsRequest : APIRequest<List<APIChatChannel>>
{
    protected override string Path => "/chat/channels";
}

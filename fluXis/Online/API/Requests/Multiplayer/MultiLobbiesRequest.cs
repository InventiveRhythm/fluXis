using System.Collections.Generic;
using fluXis.Online.API.Models.Multi;

namespace fluXis.Online.API.Requests.Multiplayer;

public class MultiLobbiesRequest : APIRequest<List<MultiplayerRoom>>
{
    protected override string Path => "/multi/lobbies";
}

using System.Collections.Generic;
using fluXis.Game.Online.API.Models.Multi;

namespace fluXis.Game.Online.API.Requests.Multiplayer;

public class MultiLobbiesRequest : APIRequest<List<MultiplayerRoom>>
{
    protected override string Path => "/multi/lobbies";
}

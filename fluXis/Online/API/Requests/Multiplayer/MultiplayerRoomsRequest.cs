using fluXis.Online.API.Models.Multi;

namespace fluXis.Online.API.Requests.Multiplayer;

public class MultiplayerRoomsRequest : APIRequest<MultiplayerRoom[]>
{
    protected override string Path => "/multiplayer/rooms";
}

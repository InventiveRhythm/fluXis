using fluXis.Game.Online.API.Models.Other;
using fluXis.Game.Online.Fluxel;
using Newtonsoft.Json;

namespace fluXis.Game.Online.API.Packets.Other;

public class ServerMessagePacket : IPacket
{
    public string ID => PacketIDs.SERVER_MESSAGE;

    [JsonProperty("message")]
    public ServerMessage Message { get; set; }

    public ServerMessagePacket(ServerMessage message)
    {
        Message = message;
    }
}

using fluXis.Shared.Components.Other;
using Newtonsoft.Json;

namespace fluXis.Shared.API.Packets.Other;

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

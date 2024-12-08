using fluXis.Game.Online.Fluxel;
using Newtonsoft.Json;

namespace fluXis.Game.Online.API.Packets.Chat;

#nullable enable

public class ChatDeletePacket : IPacket
{
    public string ID => PacketIDs.CHAT_DELETE;

    [JsonProperty("channel")]
    public string? Channel { get; set; }

    [JsonProperty("id")]
    public string? MessageID { get; set; }

    public static ChatDeletePacket Create(string channel, string id) => new() { Channel = channel, MessageID = id };
}

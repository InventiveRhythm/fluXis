using Newtonsoft.Json;

namespace fluXis.Game.Online.Fluxel.Packets.Chat;

public class ChatMessageDeletePacket : Packet
{
    [JsonProperty("id")]
    public string Id { get; set; }

    public ChatMessageDeletePacket()
        : base(12)
    {
    }
}

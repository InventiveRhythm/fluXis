using Newtonsoft.Json;

namespace fluXis.Game.Online.Fluxel.Packets.Chat;

public class ChatMessagePacket : Packet
{
    [JsonProperty("content")]
    public string Content { get; set; }

    [JsonProperty("channel")]
    public string Channel { get; set; }

    public ChatMessagePacket()
        : base(10)
    {
    }
}

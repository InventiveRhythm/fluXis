using Newtonsoft.Json;

namespace fluXis.Game.Online.Fluxel.Packets.Chat;

public class ChatHistoryPacket : Packet
{
    public override string ID => "chat/history";

    [JsonProperty("channel")]
    public string Channel { get; init; }

    public ChatHistoryPacket(string channel)
    {
        Channel = channel;
    }
}

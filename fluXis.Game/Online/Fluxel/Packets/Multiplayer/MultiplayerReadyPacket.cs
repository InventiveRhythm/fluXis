using Newtonsoft.Json;

namespace fluXis.Game.Online.Fluxel.Packets.Multiplayer;

public class MultiplayerReadyPacket : Packet
{
    public override string ID => "multi/ready";

    [JsonProperty("ready")]
    public bool Ready { get; set; }

    public MultiplayerReadyPacket(bool state)
    {
        Ready = state;
    }
}

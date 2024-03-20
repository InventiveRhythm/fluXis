using Newtonsoft.Json;

namespace fluXis.Shared.Components.Multi;

[JsonObject(MemberSerialization.OptIn)]
public interface IMultiplayerParticipant
{
    [JsonProperty("id")]
    public long ID { get; init; }

    [JsonProperty("state")]
    public MultiplayerUserState State { get; set; }
}

using Newtonsoft.Json;

namespace fluXis.Shared.Components.Maps;

public interface IAPIMapShort
{
    [JsonProperty("id")]
    long ID { get; init; }
}

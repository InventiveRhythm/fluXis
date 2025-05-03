using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using fluXis.Map.Structures.Bases;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace fluXis.Map.Structures.Events;

public class ScriptEvent : IMapEvent
{
    [JsonProperty("time")]
    public double Time { get; set; }

    [JsonProperty("path")]
    public string ScriptPath { get; set; }

    [JsonProperty("params")]
    public Dictionary<string, JToken> Parameters { get; set; } = new();

    public bool TryGetParameter<T>(string key, [NotNullWhen(true)] out T value)
    {
        value = default;

        if (!Parameters.TryGetValue(key, out var token))
            return false;

        value = token.ToObject<T>();
        return value != null;
    }
}

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using rhym;

namespace fluXis.Utils;

public class JsonResourceLocationConverter : JsonConverter<ResourceLocation>
{
    public override void WriteJson(JsonWriter writer, ResourceLocation value, JsonSerializer serializer)
        => serializer.Serialize(writer, value.ToString());

    public override ResourceLocation ReadJson(JsonReader reader, Type objectType, ResourceLocation existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var token = JToken.Load(reader);
        var value = token.ToString();

        // TODO: stupid fix to make it work with main API
        // remove this on release
        if (value.Length == 1)
            value = $"flux:keys/{value}";

        return ResourceLocation.FromString(value);
    }
}

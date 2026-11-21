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
        return ResourceLocation.FromString(value);
    }
}

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace fluXis.Utils;

public class VersionConverter : JsonConverter<Version>
{
    public override void WriteJson(JsonWriter writer, Version value, JsonSerializer serializer)
        => serializer.Serialize(writer, value.ToString());

    public override Version ReadJson(JsonReader reader, Type objectType, Version existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var token = JToken.Load(reader);
        var value = token.ToString();

        // Something like '2' wouldn't work with Version.TryParse so we have to add a period ourselves -> '2.0'
        if (!value.Contains('.'))
            value += ".0";

        return Version.TryParse(value, out var parsed)
            ? parsed
            : throw new JsonSerializationException(value);
    }
}

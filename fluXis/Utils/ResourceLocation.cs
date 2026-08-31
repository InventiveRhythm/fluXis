using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace fluXis.Utils;

[JsonConverter(typeof(JsonResourceLocationConverter))]
public readonly struct ResourceLocation : IEquatable<ResourceLocation>
{
    public string Namespace { get; }
    public string Path { get; }

    public ResourceLocation(string ns, string path)
    {
        Namespace = ns.ToLowerInvariant();
        Path = path.ToLowerInvariant();
    }

    public bool Equals(ResourceLocation other)
        => string.Equals(Namespace, other.Namespace, StringComparison.InvariantCultureIgnoreCase)
           && string.Equals(Path, other.Path, StringComparison.InvariantCultureIgnoreCase);

    public override bool Equals(object obj) => obj is ResourceLocation other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(Namespace, Path);

    public static bool operator ==(ResourceLocation left, ResourceLocation right) => left.Equals(right);
    public static bool operator !=(ResourceLocation left, ResourceLocation right) => !left.Equals(right);

    public static implicit operator ResourceLocation(string text) => FromString(text);
    public static implicit operator string(ResourceLocation rl) => rl.ToString();

    public override string ToString() => $"{Namespace}:{Path}";

    public static ResourceLocation FromString(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return default;

        var colon = value.IndexOf(':');
        if (colon == -1) throw new InvalidOperationException("ResourceLocation string does not contain a colon character.");

        string ns = value[..colon];
        string path = value[(colon + 1)..];
        return new ResourceLocation(ns, path);
    }
}

public interface IHasLocation
{
    ResourceLocation Location { get; }
}

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

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace fluXis.Shared.Utils;

public static class JsonUtils
{
    private static Dictionary<Type, Type> typeMap { get; } = new();

    public static T? Deserialize<T>(this string json) => JsonConvert.DeserializeObject<T>(json, globalSettings());
    public static string Serialize<T>(this T obj, bool indent = false) => JsonConvert.SerializeObject(obj, globalSettings(indent));

    public static void RegisterTypeConversion<T, TImpl>() where TImpl : T
        => typeMap[typeof(T)] = typeof(TImpl);

    private static JsonSerializerSettings globalSettings(bool indent = false) => new()
    {
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        Formatting = indent ? Formatting.Indented : Formatting.None,
        ObjectCreationHandling = ObjectCreationHandling.Replace,
        NullValueHandling = NullValueHandling.Ignore,
        Converters = new List<JsonConverter> { new TypeConverter() },
        ContractResolver = new ContractResolver()
    };

    private class ContractResolver : DefaultContractResolver
    {
        private static readonly Dictionary<string, string> mapping = new()
        {
            { "Time", "time" },
            { "Lane", "lane" },
            { "HoldTime", "length" },
            { "HitSound", "sound" },
            { "Type", "type" },
            { "BPM", "bpm" },
            { "Signature", "signature" },
            { "HideLines", "hide-lines" },
            { "Multiplier", "multiplier" }
        };

        protected override string ResolvePropertyName(string propertyName)
        {
            return mapping.TryGetValue(propertyName, out var value) ? value : base.ResolvePropertyName(propertyName);
        }
    }

    private class TypeConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            var type = typeMap.GetValueOrDefault(objectType, typeof(object));
            return serializer.Deserialize(reader, type);
        }

        public override bool CanConvert(Type objectType) => typeMap.ContainsKey(objectType);
    }
}

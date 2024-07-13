using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace fluXis.Shared.Utils;

public static class JsonUtils
{
    public const string JSON_CONSTRUCTOR_ERROR = "This constructor is for json parsing only.";

    private static Dictionary<Type, Type> typeMap { get; } = new();

    public static JObject Copy(this JObject obj) => JObject.Parse(obj.ToString());
    public static T? JsonCopy<T>(this T obj) => obj.Serialize().Deserialize<T>();

    public static T? Deserialize<T>(this string json) => JsonConvert.DeserializeObject<T>(json, globalSettings());
    public static string Serialize<T>(this T obj, bool indent = false) => JsonConvert.SerializeObject(obj, globalSettings(indent));

    public static bool TryDeserialize<T>(this string json, [NotNullWhen(true)] out T? obj)
    {
        try
        {
            obj = json.Deserialize<T>();
            return obj is not null;
        }
        catch
        {
            obj = default;
            return false;
        }
    }

    public static void RegisterTypeConversion<T, TImpl>() where TImpl : T
        => typeMap[typeof(T)] = typeof(TImpl);

    private static JsonSerializerSettings globalSettings(bool indent = false) => new()
    {
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        Formatting = indent ? Formatting.Indented : Formatting.None,
        ObjectCreationHandling = ObjectCreationHandling.Replace,
        NullValueHandling = NullValueHandling.Ignore,
        Converters = new List<JsonConverter> { new TypeConverter() }
    };

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

using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace fluXis.Utils;

#nullable enable

public static class JsonUtils
{
    public const string JSON_CONSTRUCTOR_ERROR = "This constructor is for json parsing only.";

    public static JObject Copy(this JObject obj) => JObject.Parse(obj.ToString());
    public static T? JsonCopy<T>(this T obj) => obj.Serialize().Deserialize<T>();

    public static T? Deserialize<T>(this string json) => JsonConvert.DeserializeObject<T>(json, GlobalSettings());
    public static string Serialize<T>(this T obj, bool indent = false) => JsonConvert.SerializeObject(obj, GlobalSettings(indent));

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

    public static JsonSerializerSettings GlobalSettings(bool indent = false) => new()
    {
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        Formatting = indent ? Formatting.Indented : Formatting.None,
        ObjectCreationHandling = ObjectCreationHandling.Replace,
        NullValueHandling = NullValueHandling.Ignore
    };
}

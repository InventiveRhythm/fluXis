using Newtonsoft.Json;

namespace fluXis.Game.Utils;

public static class JsonUtils
{
    public static T Deserialize<T>(this string json) => JsonConvert.DeserializeObject<T>(json, globalSettings());
    public static void DeserializeInto<T>(this string json, T target) => JsonConvert.PopulateObject(json, target, globalSettings());

    public static string Serialize<T>(this T obj, bool indent = false) => JsonConvert.SerializeObject(obj, globalSettings(indent));

    private static JsonSerializerSettings globalSettings(bool indent = false) => new()
    {
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        Formatting = indent ? Formatting.Indented : Formatting.None,
        ObjectCreationHandling = ObjectCreationHandling.Replace,
        NullValueHandling = NullValueHandling.Ignore,
        DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate
    };
}

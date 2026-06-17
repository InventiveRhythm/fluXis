using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace fluXis.Online.API.Models.Users;

public class APIActivity
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("data")]
    public JObject Data { get; set; }

    public static APIActivity Online => new() { Name = "Online", Data = new JObject() };
    public static APIActivity Offline => new() { Name = "Offline", Data = new JObject() };
}

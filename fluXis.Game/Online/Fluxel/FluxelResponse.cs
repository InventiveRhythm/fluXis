using fluXis.Game.Online.API;
using Newtonsoft.Json;

namespace fluXis.Game.Online.Fluxel
{
    public class FluxelResponse<T> : APIResponse<T>
    {
        [JsonProperty("id")]
        public int ID;

        public static FluxelResponse<T> Parse(string json)
        {
            return JsonConvert.DeserializeObject<FluxelResponse<T>>(json);
        }
    }
}

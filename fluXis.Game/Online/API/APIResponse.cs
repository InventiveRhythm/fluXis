using Newtonsoft.Json;

namespace fluXis.Game.Online.API
{
    public class APIResponse<T>
    {
        [JsonProperty("code")]
        public int Status;

        [JsonProperty("message")]
        public string Message;

        [JsonProperty("data")]
        public T Data;
    }
}

using Newtonsoft.Json;

namespace fluXis.Game.Online.API
{
    public class APIResponse<T>
    {
        public int Status;
        public string Message;
        public dynamic Data;

        public T GetResponse()
        {
            return JsonConvert.DeserializeObject<T>(Data.ToString());
        }
    }
}

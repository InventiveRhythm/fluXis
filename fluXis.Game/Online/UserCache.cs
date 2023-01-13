using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using fluXis.Game.Online.API;
using Newtonsoft.Json;
using osu.Framework.Logging;

namespace fluXis.Game.Online
{
    public class UserCache
    {
        private static readonly Dictionary<int, APIUser> users = new Dictionary<int, APIUser>();

        public static APIUser GetUser(int id)
        {
            if (users.ContainsKey(id))
                return users[id];

            APIUser user = loadUser(id) ?? APIUser.DummyUser(id);
            users.Add(id, user);
            return user;
        }

        private static APIUser loadUser(int id)
        {
            try
            {
                var res = Fluxel.Fluxel.Http.Send(new HttpRequestMessage(HttpMethod.Get, $"{APIConstants.API_URL}/user/{id}"));
                var data = new StreamReader(res.Content.ReadAsStream()).ReadToEnd();
                APIResponse<APIUser> user = JsonConvert.DeserializeObject<APIResponse<APIUser>>(data);

                if (user.Status == 200)
                    return user.GetResponse();

                return null;
            }
            catch (Exception e)
            {
                Logger.Error(e, "Failed to load user from API");
                return null;
            }
        }
    }
}

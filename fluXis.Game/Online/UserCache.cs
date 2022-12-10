using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
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
                var req = WebRequest.Create($"{APIConstants.API_URL}/user/{id}");
                req.Method = "GET";
                var res = new StreamReader(req.GetResponse().GetResponseStream()).ReadToEnd();
                APIResponse<APIUser> user = JsonConvert.DeserializeObject<APIResponse<APIUser>>(res);

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

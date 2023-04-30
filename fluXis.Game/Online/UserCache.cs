using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using fluXis.Game.Online.API;
using Newtonsoft.Json;
using osu.Framework.Logging;

namespace fluXis.Game.Online;

public class UserCache
{
    private static readonly Dictionary<int, APIUser> users = new();

    public static APIUser GetUser(int id)
    {
        if (users.TryGetValue(id, out var u))
            return u;

        APIUser user = loadUser(id) ?? APIUser.DummyUser(id);
        users.Add(id, user);
        return user;
    }

    private static APIUser loadUser(int id)
    {
        try
        {
            var res = Fluxel.Fluxel.Http.Send(new HttpRequestMessage(HttpMethod.Get, $"{APIConstants.APIUrl}/user/{id}"));
            var data = new StreamReader(res.Content.ReadAsStream()).ReadToEnd();
            APIResponse<APIUser> user = JsonConvert.DeserializeObject<APIResponse<APIUser>>(data);

            return user.Status == 200 ? user.Data : null;
        }
        catch (Exception e)
        {
            Logger.Error(e, "Failed to load user from API");
            return null;
        }
    }
}

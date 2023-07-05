using System;
using System.Collections.Generic;
using fluXis.Game.Online.API;
using Newtonsoft.Json;
using osu.Framework.IO.Network;
using osu.Framework.Logging;

namespace fluXis.Game.Online;

public class UserCache
{
    private static readonly Dictionary<int, APIUser> users = new();
    private static Fluxel.Fluxel fluxel;

    public static void Init(Fluxel.Fluxel api)
    {
        fluxel = api;
    }

    public static APIUser GetUser(int id, bool forceReload = false)
    {
        if (users.TryGetValue(id, out var u) && !forceReload)
            return u;

        APIUser user = fetchUser(id);
        if (user == null) return null;

        users[id] = user;
        return user;
    }

    private static APIUser fetchUser(int id)
    {
        try
        {
            var req = new WebRequest($"{fluxel.Endpoint.APIUrl}/user/{id}");
            req.AllowInsecureRequests = true;
            req.Perform();
            APIResponse<APIUser> user = JsonConvert.DeserializeObject<APIResponse<APIUser>>(req.GetResponseString());

            if (user.Status == 200) return user.Data;

            Logger.Log($"Failed to load user from API: {user.Message}", LoggingTarget.Network, LogLevel.Error);
            return null;
        }
        catch (Exception e)
        {
            Logger.Error(e, "Failed to load user from API");
            return null;
        }
    }
}

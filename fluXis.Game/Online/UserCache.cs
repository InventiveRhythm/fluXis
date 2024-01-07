using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using fluXis.Game.Online.API;
using fluXis.Game.Online.API.Models.Users;
using Newtonsoft.Json;
using osu.Framework.IO.Network;
using osu.Framework.Logging;

namespace fluXis.Game.Online;

public static class UserCache
{
    private static readonly Dictionary<int, APIUser> users = new();
    private static Fluxel.Fluxel fluxel;

    public static Action<int> OnAvatarUpdate { get; set; }
    public static Action<int> OnBannerUpdate { get; set; }

    private static readonly Dictionary<int, List<Action>> avatar_update_callbacks = new();
    private static readonly Dictionary<int, List<Action>> banner_update_callbacks = new();

    public static void Init(Fluxel.Fluxel api)
    {
        fluxel = api;
    }

    public static async Task<APIUser> GetUserAsync(int id, bool forceReload = false)
    {
        var user = await Task.Run(() => GetUser(id, forceReload));
        return user;
    }

    public static APIUser GetUser(int id, bool forceReload = false)
    {
        if (id == -1)
        {
            return new APIUser
            {
                ID = fluxel.LoggedInUser?.ID ?? -1,
                Username = fluxel.LoggedInUser?.Username ?? "Guest",
                CountryCode = fluxel.LoggedInUser?.CountryCode ?? "XX",
                Role = fluxel.LoggedInUser?.Role ?? 0
            };
        }

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

    public static List<Action> GetAvatarUpdateCallbacks(int id)
    {
        if (!avatar_update_callbacks.ContainsKey(id))
            avatar_update_callbacks[id] = new List<Action>();

        return avatar_update_callbacks[id];
    }

    public static void TriggerAvatarUpdate(int id)
    {
        OnAvatarUpdate?.Invoke(id);

        if (!avatar_update_callbacks.ContainsKey(id)) return;

        foreach (var callback in avatar_update_callbacks[id])
            callback.Invoke();
    }

    public static List<Action> GetBannerUpdateCallbacks(int id)
    {
        if (!banner_update_callbacks.ContainsKey(id))
            banner_update_callbacks[id] = new List<Action>();

        return banner_update_callbacks[id];
    }

    public static void TriggerBannerUpdate(int id)
    {
        OnBannerUpdate?.Invoke(id);

        if (!banner_update_callbacks.ContainsKey(id)) return;

        foreach (var callback in banner_update_callbacks[id])
            callback.Invoke();
    }
}

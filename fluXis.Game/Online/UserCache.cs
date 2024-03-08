using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using fluXis.Game.Online.API.Models.Users;
using fluXis.Game.Online.API.Requests.Users;
using fluXis.Game.Online.Fluxel;
using osu.Framework.Logging;

namespace fluXis.Game.Online;

public static class UserCache
{
    private static readonly Dictionary<long, APIUser> users = new();
    private static FluxelClient fluxel;

    public static Action<long> OnAvatarUpdate { get; set; }
    public static Action<long> OnBannerUpdate { get; set; }

    private static readonly Dictionary<long, List<Action>> avatar_update_callbacks = new();
    private static readonly Dictionary<long, List<Action>> banner_update_callbacks = new();

    public static void Init(FluxelClient api)
    {
        fluxel = api;
    }

    public static async Task<APIUser> GetUserAsync(long id, bool forceReload = false)
    {
        var user = await Task.Run(() => GetUser(id, forceReload));
        return user;
    }

    public static APIUser GetUser(long id, bool forceReload = false)
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

    private static APIUser fetchUser(long id)
    {
        try
        {
            var req = new UserRequest(id);
            req.Perform(fluxel);
            var res = req.Response;

            if (res.Status == 200)
                return res.Data;

            Logger.Log($"Failed to load user from API: {res.Message}", LoggingTarget.Network, LogLevel.Error);
            return null;
        }
        catch (Exception e)
        {
            Logger.Error(e, "Failed to load user from API");
            return null;
        }
    }

    public static List<Action> GetAvatarUpdateCallbacks(long id)
    {
        if (!avatar_update_callbacks.ContainsKey(id))
            avatar_update_callbacks[id] = new List<Action>();

        return avatar_update_callbacks[id];
    }

    public static void TriggerAvatarUpdate(long id)
    {
        OnAvatarUpdate?.Invoke(id);

        if (!avatar_update_callbacks.ContainsKey(id)) return;

        foreach (var callback in avatar_update_callbacks[id])
            callback.Invoke();
    }

    public static List<Action> GetBannerUpdateCallbacks(long id)
    {
        if (!banner_update_callbacks.ContainsKey(id))
            banner_update_callbacks[id] = new List<Action>();

        return banner_update_callbacks[id];
    }

    public static void TriggerBannerUpdate(long id)
    {
        OnBannerUpdate?.Invoke(id);

        if (!banner_update_callbacks.ContainsKey(id)) return;

        foreach (var callback in banner_update_callbacks[id])
            callback.Invoke();
    }
}

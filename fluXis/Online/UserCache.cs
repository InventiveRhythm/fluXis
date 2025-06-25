using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using fluXis.Online.API.Models.Users;
using fluXis.Online.API.Requests.Users;
using fluXis.Online.Fluxel;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Logging;

namespace fluXis.Online;

public partial class UserCache : Component
{
    [Resolved]
    private IAPIClient api { get; set; }

    private Dictionary<long, APIUser> users { get; } = new();

    private readonly object @lock = new();

    private Dictionary<long, List<Action<string>>> avatarUpdateCallbacks { get; } = new();
    private Dictionary<long, List<Action<string>>> bannerUpdateCallbacks { get; } = new();

    public async Task<APIUser> UserAsync(long id, bool forceReload = false)
    {
        var user = await Task.Run(() => Get(id, forceReload));
        return user;
    }

    public APIUser Get(long id, bool forceReload = false)
    {
        if (id == -1)
        {
            return new APIUser
            {
                ID = api.User.Value?.ID ?? -1,
                Username = api.User.Value?.Username ?? "Guest",
                CountryCode = api.User.Value?.CountryCode ?? "XX"
            };
        }

        if (users.TryGetValue(id, out var u) && !forceReload)
            return u;

        APIUser user = fetch(id);
        if (user == null) return null;

        lock (@lock)
            users[id] = user;

        return user;
    }

    [CanBeNull]
    private APIUser fetch(long id)
    {
        if (!api.CanUseOnline)
            return null;

        try
        {
            var req = new UserRequest(id);
            api.PerformRequest(req);

            if (req.IsSuccessful)
                return req.Response!.Data;

            Logger.Log($"Failed to load user from API: {req.FailReason?.Message}", LoggingTarget.Network, LogLevel.Error);
            return null;
        }
        catch (Exception e)
        {
            Logger.Error(e, "Failed to load user from API");
            return null;
        }
    }

    public void TriggerAvatarUpdate(long id, string hash)
    {
        lock (@lock)
        {
            if (!avatarUpdateCallbacks.TryGetValue(id, out var callbacks))
                return;

            callbacks.ForEach(c => c.Invoke(hash));
        }
    }

    public void TriggerBannerUpdate(long id, string hash)
    {
        lock (@lock)
        {
            if (!bannerUpdateCallbacks.TryGetValue(id, out var callbacks))
                return;

            callbacks.ForEach(c => c.Invoke(hash));
        }
    }

    public void RegisterAvatarCallback(long id, Action<string> callback)
    {
        lock (@lock)
        {
            if (!avatarUpdateCallbacks.ContainsKey(id))
                avatarUpdateCallbacks[id] = new List<Action<string>>();

            avatarUpdateCallbacks[id].Add(callback);
        }
    }

    public void RegisterBannerCallback(long id, Action<string> callback)
    {
        lock (@lock)
        {
            if (!bannerUpdateCallbacks.ContainsKey(id))
                bannerUpdateCallbacks[id] = new List<Action<string>>();

            bannerUpdateCallbacks[id].Add(callback);
        }
    }

    public void UnregisterAvatarCallback(long id, Action<string> callback)
    {
        lock (@lock)
        {
            if (!avatarUpdateCallbacks.TryGetValue(id, out var callbacks))
                return;

            callbacks.Remove(callback);
        }
    }

    public void UnregisterBannerCallback(long id, Action<string> callback)
    {
        lock (@lock)
        {
            if (!bannerUpdateCallbacks.TryGetValue(id, out var callbacks))
                return;

            callbacks.Remove(callback);
        }
    }
}

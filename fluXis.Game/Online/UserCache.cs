using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using fluXis.Game.Online.API.Requests.Users;
using fluXis.Game.Online.Fluxel;
using fluXis.Shared.Components.Users;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Logging;

namespace fluXis.Game.Online;

public partial class UserCache : Component
{
    [Resolved]
    private IAPIClient api { get; set; }

    private Dictionary<long, APIUser> users { get; } = new();

    private readonly object dictLock = new();

    public event Action<long> OnAvatarUpdate;
    public event Action<long> OnBannerUpdate;

    private Dictionary<long, List<Action>> avatarUpdateCallbacks { get; } = new();
    private Dictionary<long, List<Action>> bannerUpdateCallbacks { get; } = new();

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

        lock (dictLock)
            users[id] = user;

        return user;
    }

    private APIUser fetch(long id)
    {
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

    public void TriggerAvatarUpdate(long id)
    {
        OnAvatarUpdate?.Invoke(id);

        if (!avatarUpdateCallbacks.TryGetValue(id, out var callbacks))
            return;

        callbacks.ForEach(c => c.Invoke());
    }

    public void TriggerBannerUpdate(long id)
    {
        OnBannerUpdate?.Invoke(id);

        if (!bannerUpdateCallbacks.TryGetValue(id, out var callbacks))
            return;

        callbacks.ForEach(c => c.Invoke());
    }

    public void RegisterAvatarCallback(long id, Action callback)
    {
        if (!avatarUpdateCallbacks.ContainsKey(id))
            avatarUpdateCallbacks[id] = new List<Action>();

        avatarUpdateCallbacks[id].Add(callback);
    }

    public void RegisterBannerCallback(long id, Action callback)
    {
        if (!bannerUpdateCallbacks.ContainsKey(id))
            bannerUpdateCallbacks[id] = new List<Action>();

        bannerUpdateCallbacks[id].Add(callback);
    }

    public void UnregisterAvatarCallback(long id, Action callback)
    {
        if (!avatarUpdateCallbacks.TryGetValue(id, out var callbacks))
            return;

        callbacks.Remove(callback);
    }

    public void UnregisterBannerCallback(long id, Action callback)
    {
        if (!bannerUpdateCallbacks.TryGetValue(id, out var callbacks))
            return;

        callbacks.Remove(callback);
    }
}

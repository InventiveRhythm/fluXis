using System;
using System.Collections.Generic;
using fluXis.Online.Fluxel;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Logging;
using Steamworks;

namespace fluXis.Integration;

public partial class SteamManager : Component
{
    [Resolved]
    private IAPIClient api { get; set; }

    public bool Initialized { get; }

    private Dictionary<string, string> rpc { get; } = new();

    public SteamManager()
    {
        try
        {
            if (!(Initialized = SteamAPI.Init()))
                throw new Exception("SteamAPI.Init() failed.");
        }
        catch (Exception e)
        {
            Logger.Error(e, "Failed to connect to steam client!");
        }
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        if (!Initialized)
            return;

        api.Activity.BindValueChanged(v =>
        {
            var activity = v.NewValue;

            if (activity is null)
                return;

            SteamFriends.ClearRichPresence();
            rpc.Clear();

            activity.CreateSteam(this);

            var hasDetails = rpc.Keys.Contains("details");
            SteamFriends.SetRichPresence("steam_display", hasDetails ? "#WithDetails" : "#BasicStatus");
        }, true);
    }

    public void OpenLink(string url) => SteamFriends.ActivateGameOverlayToWebPage(url);

    public void SetRichPresence(SteamRichPresenceKey key, string value)
    {
        var pchKey = key switch
        {
            SteamRichPresenceKey.Status => "status",
            SteamRichPresenceKey.Details => "details",
            _ => throw new ArgumentOutOfRangeException(nameof(key), key, null)
        };

        SteamFriends.SetRichPresence(pchKey, value);
        rpc[pchKey] = value;
    }
}

public enum SteamRichPresenceKey
{
    Status,
    Details
}

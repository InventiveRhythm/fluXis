using System;
using System.Collections.Generic;
using fluXis.Online.API.Requests.Users;
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

    private Logger logger { get; } = Logger.GetLogger("Steam");
    private Dictionary<string, string> rpc { get; } = new();
    private double lastUpdate;

    private Callback<GetTicketForWebApiResponse_t> ticketCb { get; }

    public SteamManager()
    {
        try
        {
            Initialized = SteamAPI.Init();

            if (!Initialized)
                throw new Exception("SteamAPI.Init() failed.");

            ticketCb = Callback<GetTicketForWebApiResponse_t>.Create(authTicketCallback);
        }
        catch (Exception e)
        {
            logger.Add("Failed to connect to steam client!", LogLevel.Error, e);
        }
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        if (!Initialized)
            return;

        logger.Add($"Logged in through steam as {SteamFriends.GetPersonaName()} [{SteamUser.GetSteamID().m_SteamID}]");

        api.Status.BindValueChanged(v =>
        {
            if (v.NewValue == ConnectionStatus.Online) startAccountLink();
        }, true);

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

    protected override void Update()
    {
        base.Update();

        if (!Initialized)
            return;

        var delta = Time.Current - lastUpdate;

        if (delta < 50)
            return;

        lastUpdate = Time.Current;
        SteamAPI.RunCallbacks();
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

    private void startAccountLink()
    {
        if (api.User.Value is null || api.User.Value.SteamID is not null)
            return;

        logger.Add("Linking accounts...");
        SteamUser.GetAuthTicketForWebApi(null);
    }

    private void authTicketCallback(GetTicketForWebApiResponse_t ticket)
    {
        if (ticket.m_eResult != EResult.k_EResultOK)
        {
            logger.Add($"Failed to get auth ticket! [{ticket.m_eResult}]", LogLevel.Error);
            return;
        }

        logger.Add($"Received ticket. [{ticket.m_cubTicket}]");

        var bytes = ticket.m_rgubTicket;
        var str = BitConverter.ToString(bytes).Replace("-", "").ToLower();

        var req = new UserConnectionCreateRequest(api.User.Value.ID, "steam", str);
        req.Success += res => api.User.Value.SteamID = res.Data?.ToObject<ulong>() ?? 0;
        req.Failure += ex => logger.Add("Failed to link account!", LogLevel.Error, ex);
        api.PerformRequestAsync(req);
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        ticketCb?.Dispose();
    }
}

public enum SteamRichPresenceKey
{
    Status,
    Details
}

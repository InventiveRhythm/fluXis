using System;
using System.Collections.Generic;
using System.IO;
using fluXis.Integration;
using fluXis.Online.API.Requests.Users;
using fluXis.Online.Fluxel;
using fluXis.Skinning;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Logging;
using Steamworks;

namespace fluXis.Desktop.Integration;

public partial class SteamManager : Component, ISteamManager
{
    [Resolved]
    private IAPIClient api { get; set; }

    [Resolved]
    private SkinManager skinManager { get; set; }

    public uint AppID => 3440100;
    public bool Initialized { get; }
    public List<PublishedFileId_t> WorkshopItems { get; }

    private Logger logger { get; } = Logger.GetLogger("Steam");
    private Dictionary<string, string> rpc { get; } = new();
    private double lastUpdate;

    private Callback<GetTicketForWebApiResponse_t> ticketCb { get; }
    private CallResult<CreateItemResult_t> createItemCb { get; }

    [CanBeNull]
    private IWorkshopItem currentItem;

    public SteamManager()
    {
        try
        {
            File.WriteAllText("steam_appid.txt", AppID.ToString());
            Initialized = SteamAPI.Init();

            if (!Initialized)
                throw new Exception("SteamAPI.Init() failed.");

            ticketCb = Callback<GetTicketForWebApiResponse_t>.Create(authTicketCallback);
            createItemCb = CallResult<CreateItemResult_t>.Create(createItemCallback);

            var num = SteamUGC.GetNumSubscribedItems();
            var items = new PublishedFileId_t[num];
            var result = SteamUGC.GetSubscribedItems(items, num);

            logger.Add($"Found {items.Length} subscribed items. [subscribed: {num}, result: {result}]");

            WorkshopItems = new List<PublishedFileId_t>(items);
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

    public void UploadItem(IWorkshopItem item)
    {
        currentItem = item;
        logger.Add($"Uploading item: {item}.");

        var handle = SteamUGC.CreateItem((AppId_t)AppID, EWorkshopFileType.k_EWorkshopFileTypeCommunity);
        createItemCb.Set(handle);
    }

    public void UpdateItem(PublishedFileId_t id, IWorkshopItem item)
    {
        logger.Add($"Updating item: {item}.");
        var handle = SteamUGC.StartItemUpdate((AppId_t)AppID, id);

        SteamUGC.SetItemTitle(handle, item.Title);

        if (!string.IsNullOrWhiteSpace(item.Preview) && File.Exists(item.Preview))
            SteamUGC.SetItemPreview(handle, item.Preview);

        SteamUGC.SetItemContent(handle, item.Folder);
        SteamUGC.SubmitItemUpdate(handle, "");

        OpenLink($"https://steamcommunity.com/sharedfiles/filedetails/?id={id.m_PublishedFileId}");
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

        if (api.User.Value is null)
            return;

        logger.Add($"Received ticket. [{ticket.m_cubTicket}]");

        var bytes = ticket.m_rgubTicket;
        var str = BitConverter.ToString(bytes).Replace("-", "").ToLower();

        var req = new UserConnectionCreateRequest(api.User.Value.ID, "steam", str);
        req.Success += res => api.User.Value.SteamID = res.Data?.ToObject<ulong>() ?? 0;
        req.Failure += ex => logger.Add("Failed to link account!", LogLevel.Error, ex);
        api.PerformRequestAsync(req);
    }

    private void createItemCallback(CreateItemResult_t result, bool biofail)
    {
        if (result.m_eResult != EResult.k_EResultOK)
        {
            logger.Add($"Failed to create item! [{result.m_eResult}]", LogLevel.Error);
            return;
        }

        logger.Add($"Created item! [{result.m_nPublishedFileId}]");

        if (currentItem is null)
            throw new InvalidOperationException("Current item is null!");

        Logger.Log(Path.Combine(currentItem.Folder, "workshopid.txt"));
        File.WriteAllText(Path.Combine(currentItem.Folder, "workshopid.txt"), result.m_nPublishedFileId.ToString());

        UpdateItem(result.m_nPublishedFileId, currentItem);
        currentItem = null;
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        if (Initialized)
            SteamAPI.Shutdown();

        ticketCb?.Dispose();
        createItemCb?.Dispose();
    }
}

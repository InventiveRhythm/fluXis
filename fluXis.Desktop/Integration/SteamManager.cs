using System;
using System.Collections.Generic;
using System.IO;
using fluXis.Graphics.UserInterface.Text;
using fluXis.Integration;
using fluXis.Online.API.Requests.Users;
using fluXis.Online.Fluxel;
using fluXis.Overlay.Notifications;
using fluXis.Overlay.Notifications.Tasks;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Extensions.EnumExtensions;
using osu.Framework.Graphics;
using osu.Framework.Logging;
using Steamworks;

namespace fluXis.Desktop.Integration;

public partial class SteamManager : Component, ISteamManager
{
    [Resolved]
    private IAPIClient api { get; set; }

    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private NotificationManager notifications { get; set; }

    public const int APP_ID = 3440100;
    public uint AppID => APP_ID;
    public bool Initialized { get; }

    private Logger logger { get; } = Logger.GetLogger("Steam");
    private Dictionary<string, string> rpc { get; } = new();
    private double lastUpdate;

    private Callback<GetTicketForWebApiResponse_t> ticketCb { get; }

    public SteamManager()
    {
        try
        {
            File.WriteAllText("steam_appid.txt", AppID.ToString());
            Initialized = SteamAPI.Init();

            if (!Initialized)
                throw new Exception("SteamAPI.Init() failed.");

            ticketCb = Callback<GetTicketForWebApiResponse_t>.Create(authTicketCallback);

            subscribedListChangedCb = Callback<UserSubscribedItemsListChanged_t>.Create(subscribedItemsChanged);
            createItemCr = CallResult<CreateItemResult_t>.Create(createItemCallback);
            submitItemCr = CallResult<SubmitItemUpdateResult_t>.Create(onItemSubmitted);

            keyboardClose = Callback<FloatingGamepadTextInputDismissed_t>.Create(onKeyboardClosed);

            syncWorkshopItems();
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

        if (delta < 16)
            return;

        lastUpdate = Time.Current;
        SteamAPI.RunCallbacks();
        updateDownloadProgress();
    }

    #region Workshop

    public List<ulong> WorkshopItems { get; } = [];
    private readonly Dictionary<PublishedFileId_t, TaskNotificationData> downloadProgress = [];
    private readonly Dictionary<PublishedFileId_t, SteamUGCDetails_t> itemInfo = [];

    public event Action<bool> ItemCreated;
    public event Action<bool> ItemUpdated;
    public event Action ItemListUpdated;

    [CanBeNull]
    private IWorkshopItem currentItem;

    private readonly Callback<UserSubscribedItemsListChanged_t> subscribedListChangedCb;
    private readonly CallResult<CreateItemResult_t> createItemCr;
    private readonly CallResult<SubmitItemUpdateResult_t> submitItemCr;

    [CanBeNull]
    private CallResult<SteamUGCQueryCompleted_t> queryCompleteCr;

    private void requestItemDetails(PublishedFileId_t id)
    {
        var query = SteamUGC.CreateQueryUGCDetailsRequest([id], 1);
        SteamUGC.SetReturnLongDescription(query, true);

        var call = SteamUGC.SendQueryUGCRequest(query);

        queryCompleteCr ??= CallResult<SteamUGCQueryCompleted_t>.Create(receiveItemDetails);
        queryCompleteCr?.Set(call);
    }

    private void receiveItemDetails(SteamUGCQueryCompleted_t complete, bool fail)
    {
        if (fail || complete.m_eResult != EResult.k_EResultOK)
        {
            SteamUGC.ReleaseQueryUGCRequest(complete.m_handle);
            return;
        }

        for (uint i = 0; i < complete.m_unNumResultsReturned; i++)
        {
            if (!SteamUGC.GetQueryUGCResult(complete.m_handle, i, out var details)) continue;
            if (details.m_eResult != EResult.k_EResultOK) continue;

            itemInfo[details.m_nPublishedFileId] = details;
        }

        SteamUGC.ReleaseQueryUGCRequest(complete.m_handle);
    }

    private void syncWorkshopItems()
    {
        var num = SteamUGC.GetNumSubscribedItems();
        var items = new PublishedFileId_t[num];

        SteamUGC.GetSubscribedItems(items, num);
        WorkshopItems.Clear();

        foreach (var item in items)
        {
            var state = (EItemState)SteamUGC.GetItemState(item);

            var downloading = state.HasFlagFast(EItemState.k_EItemStateDownloading);
            var needsUpdate = state.HasFlagFast(EItemState.k_EItemStateNeedsUpdate);
            var installed = state.HasFlagFast(EItemState.k_EItemStateInstalled);

            if (downloading || needsUpdate || !installed)
            {
                startDownload(item);

                // force steam to download the item if it didn't start it automatically
                if (!downloading) SteamUGC.DownloadItem(item, false);
            }
            else
            {
                WorkshopItems.Add(item.m_PublishedFileId);
            }
        }

        void startDownload(PublishedFileId_t item)
        {
            if (downloadProgress.ContainsKey(item))
                return;

            requestItemDetails(item);

            var notification = new TaskNotificationData { Text = "Downloading workshop item..." };
            downloadProgress[item] = notification;
            notifications?.AddTask(notification);
        }
    }

    private void updateDownloadProgress()
    {
        var complete = new List<PublishedFileId_t>();

        foreach (var (id, task) in downloadProgress)
        {
            var state = (EItemState)SteamUGC.GetItemState(id);

            if (itemInfo.TryGetValue(id, out var details))
            {
                var text = $"Downloading '{details.m_rgchTitle}'...";

                if (text != task.Text)
                {
                    task.Text = text;
                    task.TriggerTextUpdate();
                }
            }

            if (state.HasFlagFast(EItemState.k_EItemStateDownloadPending) && !state.HasFlagFast(EItemState.k_EItemStateDownloading))
                continue;

            if (state.HasFlagFast(EItemState.k_EItemStateDownloading))
            {
                if (SteamUGC.GetItemDownloadInfo(id, out ulong current, out ulong total))
                {
                    if (total <= 0)
                        continue;

                    task.Progress = current / (float)total;
                }
            }
            else
            {
                if (state.HasFlagFast(EItemState.k_EItemStateInstalled))
                    WorkshopItems.Add(id.m_PublishedFileId);

                complete.Add(id);
            }
        }

        complete.ForEach(x =>
        {
            downloadProgress[x].State = LoadingState.Complete;
            downloadProgress.Remove(x);
        });
    }

    public void UploadItem(IWorkshopItem item)
    {
        currentItem = item;
        logger.Add($"Uploading item: {item}.");

        var handle = SteamUGC.CreateItem((AppId_t)AppID, EWorkshopFileType.k_EWorkshopFileTypeCommunity);
        createItemCr.Set(handle);
    }

    public void UpdateItem(ulong id, IWorkshopItem item)
    {
        logger.Add($"Updating item: {item}.");
        var handle = SteamUGC.StartItemUpdate((AppId_t)AppID, new PublishedFileId_t(id));

        SteamUGC.SetItemTitle(handle, item.Title);

        if (!string.IsNullOrWhiteSpace(item.Preview) && File.Exists(item.Preview))
            SteamUGC.SetItemPreview(handle, item.Preview);

        SteamUGC.SetItemContent(handle, item.Folder);
        var submitHandle = SteamUGC.SubmitItemUpdate(handle, "");
        submitItemCr.Set(submitHandle);
    }

    public string GetWorkshopItemDirectory(ulong id)
    {
        SteamUGC.GetItemInstallInfo(new PublishedFileId_t(id), out ulong _, out string folder, 2048, out _);
        return folder;
    }

    #region Callbacks

    private void subscribedItemsChanged(UserSubscribedItemsListChanged_t change)
    {
        if (change.m_nAppID.m_AppId != APP_ID)
            return;

        logger.Add("Items changed.");
        syncWorkshopItems();
    }

    private void createItemCallback(CreateItemResult_t result, bool biofail)
    {
        if (result.m_eResult != EResult.k_EResultOK)
        {
            logger.Add($"Failed to create item! [{result.m_eResult}]", LogLevel.Error);
            ItemCreated?.Invoke(false);
            return;
        }

        logger.Add($"Created item! [{result.m_nPublishedFileId}]");
        ItemCreated?.Invoke(true);

        if (currentItem is null)
            throw new InvalidOperationException("Current item is null!");

        Logger.Log(Path.Combine(currentItem.Folder, "workshopid.txt"));
        File.WriteAllText(Path.Combine(currentItem.Folder, "workshopid.txt"), result.m_nPublishedFileId.ToString());

        UpdateItem(result.m_nPublishedFileId.m_PublishedFileId, currentItem);
        currentItem = null;
    }

    private void onItemSubmitted(SubmitItemUpdateResult_t result, bool biofail)
    {
        if (result.m_eResult != EResult.k_EResultOK)
        {
            logger.Add($"Failed to submit item! [{result.m_eResult}]", LogLevel.Error);
            ItemUpdated?.Invoke(false);
            return;
        }

        ItemUpdated?.Invoke(true);

        OpenLink($"https://steamcommunity.com/sharedfiles/filedetails/?id={result.m_nPublishedFileId}");
    }

    #endregion

    #endregion

    #region Big Picture Keyboard

    [CanBeNull]
    private FluXisTextBox currentTextBox;

    private Callback<FloatingGamepadTextInputDismissed_t> keyboardClose { get; }

    public void OpenKeyboard(FluXisTextBox box)
    {
        currentTextBox = box;
        var size = box.ScreenSpaceDrawQuad;

        SteamUtils.ShowFloatingGamepadTextInput(
            EFloatingGamepadTextInputMode.k_EFloatingGamepadTextInputModeModeSingleLine,
            (int)size.TopLeft.X,
            (int)size.TopLeft.Y,
            (int)size.Width,
            (int)size.Height
        );
    }

    public void CloseKeyboard()
    {
        currentTextBox = null;
        SteamUtils.DismissFloatingGamepadTextInput();
    }

    private void onKeyboardClosed(FloatingGamepadTextInputDismissed_t param) => currentTextBox?.RemoveFocus();

    #endregion

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

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        if (Initialized)
            SteamAPI.Shutdown();

        ticketCb?.Dispose();
        subscribedListChangedCb?.Dispose();
        createItemCr?.Dispose();
        submitItemCr?.Dispose();
        keyboardClose?.Dispose();
    }
}

using System.Collections.Generic;
using Steamworks;

namespace fluXis.Integration;

public interface ISteamManager
{
    uint AppID { get; }
    bool Initialized { get; }

    List<PublishedFileId_t> WorkshopItems { get; }

    void OpenLink(string url);
    void SetRichPresence(SteamRichPresenceKey key, string value);

    void UploadItem(IWorkshopItem item);
    void UpdateItem(PublishedFileId_t id, IWorkshopItem item);

    public void UpdateItem(ulong id, IWorkshopItem item)
        => UpdateItem(new PublishedFileId_t(id), item);
}

public enum SteamRichPresenceKey
{
    Status,
    Details
}

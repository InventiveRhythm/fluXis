using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using osu.Framework.Graphics.Primitives;

namespace fluXis.Integration;

public interface ISteamManager
{
    uint AppID { get; }
    bool Initialized { get; }

    List<ulong> WorkshopItems { get; }

    Action<bool> ItemCreated { get; set; }
    Action<bool> ItemUpdated { get; set; }

    void OpenLink(string url);
    void SetRichPresence(SteamRichPresenceKey key, string value);

    void OpenKeyboard(Quad size);
    void CloseKeyboard();

    void UploadItem(IWorkshopItem item);
    void UpdateItem(ulong id, IWorkshopItem item);

    [CanBeNull]
    string GetWorkshopItemDirectory(ulong id);
}

public enum SteamRichPresenceKey
{
    Status,
    Details
}

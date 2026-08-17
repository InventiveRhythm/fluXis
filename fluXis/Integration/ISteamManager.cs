using System;
using System.Collections.Generic;
using fluXis.Graphics.UserInterface.Text;
using JetBrains.Annotations;

namespace fluXis.Integration;

public interface ISteamManager
{
    uint AppID { get; }
    bool Initialized { get; }

    List<ulong> WorkshopItems { get; }

    event Action<bool> ItemCreated;
    event Action<bool> ItemUpdated;
    event Action ItemListUpdated;

    void OpenLink(string url);
    void SetRichPresence(SteamRichPresenceKey key, string value);

    void OpenKeyboard(FluXisTextBox box);
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

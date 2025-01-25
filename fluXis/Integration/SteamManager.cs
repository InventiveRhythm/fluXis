using System;
using osu.Framework.Graphics;
using osu.Framework.Logging;
using Steamworks;

namespace fluXis.Integration;

public partial class SteamManager : Component
{
    public bool Initialized { get; }

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

    public void OpenLink(string url) => SteamFriends.ActivateGameOverlayToWebPage(url);
}

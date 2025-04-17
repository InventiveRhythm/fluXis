namespace fluXis.Integration;

public interface ISteamManager
{
    bool Initialized { get; }

    void OpenLink(string url);
    void SetRichPresence(SteamRichPresenceKey key, string value);
}

public enum SteamRichPresenceKey
{
    Status,
    Details
}

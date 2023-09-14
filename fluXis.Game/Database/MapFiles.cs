using osu.Framework.Platform;

namespace fluXis.Game.Database;

public static class MapFiles
{
    private static Storage storage;

    public static void Initialize(Storage storage) => MapFiles.storage = storage;
    public static string GetFullPath(string path) => storage.GetFullPath(path);
}

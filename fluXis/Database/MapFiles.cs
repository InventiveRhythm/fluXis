using fluXis.Database.Maps;
using osu.Framework.Platform;

namespace fluXis.Database;

public static class MapFiles
{
    private static Storage storage;

    public static void Initialize(Storage storage) => MapFiles.storage = storage;

    public static string GetFullPath(string path)
    {
        try
        {
            return storage.GetFullPath(path);
        }
        catch
        {
            return path;
        }
    }

    public static void PresentExternally(RealmMap map) => storage.PresentFileExternally($"{map.MapSet.ID}/{map.FileName}");
}

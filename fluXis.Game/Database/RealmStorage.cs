using osu.Framework.Platform;

namespace fluXis.Game.Database;

public static class RealmStorage
{
    private static Storage storage;

    public static void Initialize(Storage storage) => RealmStorage.storage = storage;
    public static string GetFullPath(RealmFile file) => storage.GetFullPath(file.Path);
}

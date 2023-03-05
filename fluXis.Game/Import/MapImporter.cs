using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using fluXis.Game.Database;
using fluXis.Game.Map;
using osu.Framework.Platform;

namespace fluXis.Game.Import;

public class MapImporter
{
    public FluXisRealm Realm { get; set; }
    public MapStore MapStore { get; set; }
    public Storage Storage { get; set; }

    public MapImporter(FluXisRealm realm, MapStore mapStore, Storage storage)
    {
        Realm = realm;
        MapStore = mapStore;
        Storage = storage;
    }

    public static string GetHash(ZipArchiveEntry entry)
    {
        var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(entry.Open());
        var hashString = BitConverter.ToString(hashBytes);

        return hashString.Replace("-", "").ToLower();
    }

    public void CopyFile(ZipArchiveEntry entry, string folder)
    {
        string destPath = Path.Combine(Storage.GetFullPath("import"), folder, entry.FullName);
        Directory.CreateDirectory(Path.GetDirectoryName(destPath));
        entry.ExtractToFile(destPath, true);
    }

    public void WriteFile(string content, string path)
    {
        string destPath = Path.Combine(Storage.GetFullPath("import"), path);
        Directory.CreateDirectory(Path.GetDirectoryName(destPath));
        File.WriteAllText(destPath, content);
    }
}

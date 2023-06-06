using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using fluXis.Game.Database;
using fluXis.Game.Database.Maps;
using fluXis.Game.Map;
using fluXis.Game.Overlay.Notification;
using osu.Framework.Platform;

namespace fluXis.Game.Import;

public class MapImporter
{
    public virtual string[] FileExtensions { get; } = Array.Empty<string>();
    public virtual string Name => "Unknown";
    public virtual string Author => "Unknown";
    public virtual Version Version => new(0, 0, 0);
    public virtual bool SupportsAutoImport => false;
    public virtual string Color => "#000000";
    public virtual string StoragePath => "";

    /// <summary>
    /// ID of the importer in the database.
    /// <para/>
    /// Assign this to FluXisImport.MapStatus!
    /// <para/>
    /// DO NOT SET THIS MANUALLY ELSE IT WILL BREAK SONGSELECT.
    /// </summary>
    public int ID { get; set; }

    public FluXisRealm Realm { get; set; }
    public MapStore MapStore { get; set; }
    public Storage Storage { get; set; }
    public NotificationOverlay Notifications { get; set; }

    public virtual void Import(string path) => throw new NotImplementedException();

    public virtual List<RealmMapSet> GetMaps() => new();
    public virtual string GetAsset(RealmMap map, ImportedAssetType type) => "";
    public virtual MapPackage GetMapPackage(RealmMap map) => new();

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

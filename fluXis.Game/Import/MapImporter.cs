using System;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Threading.Tasks;
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

    public virtual Task Import(string path)
    {
        throw new NotImplementedException();
    }

    public string GetHash(ZipArchiveEntry entry)
    {
        var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(entry.Open());
        var hashString = BitConverter.ToString(hashBytes);

        return hashString.Replace("-", "").ToLower();
    }
}

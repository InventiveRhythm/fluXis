using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using fluXis.Game.Import;
using fluXis.Import.Stepmania.Map;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace fluXis.Import.Stepmania;

[UsedImplicitly]
public class StepmaniaImport : MapImporter
{
    public override string[] FileExtensions => new[] { ".sm" };
    public override string Name => "Stepmania";
    public override string Author => "Flustix";
    public override Version Version => new(1, 0, 0);
    public override string Color => "#f0d01f";

    public override void Import(string path)
    {
        var folderPath = Path.GetDirectoryName(path);
        var fileName = Path.GetFileNameWithoutExtension(path);

        var storagePath = Storage.GetFullPath("import") + "/" + fileName;
        Directory.CreateDirectory(storagePath);

        var data = File.ReadAllText(path);

        var map = new StepmaniaFile();
        map.Parse(data);

        var mapInfos = map.ToMapInfos();

        foreach (var info in mapInfos)
        {
            var json = JsonConvert.SerializeObject(info);
            WriteFile(json, Path.Combine(storagePath, info.Metadata.Difficulty + ".fsc"));
        }

        var zipPath = Path.Combine(Storage.GetFullPath("import"), fileName + ".fms");
        if (File.Exists(zipPath)) File.Delete(zipPath);

        ZipArchive fms = ZipFile.Open(Path.Combine(Storage.GetFullPath("import"), fileName + ".fms"), ZipArchiveMode.Create);

        foreach (var info in mapInfos)
            fms.CreateEntryFromFile(Path.Combine(storagePath, info.Metadata.Difficulty + ".fsc"), info.Metadata.Difficulty + ".fsc");

        Directory.GetFiles(folderPath).ToList().ForEach(x =>
        {
            if (x.EndsWith(".sm")) return;

            var name = Path.GetFileName(x);
            fms.CreateEntryFromFile(x, name);
        });

        fms.Dispose();

        var import = new FluXisImport
        {
            MapStatus = ID,
            Notification = null,
            Realm = Realm,
            MapStore = MapStore,
            Storage = Storage,
            Notifications = Notifications
        };
        import.Import(Path.Combine(Storage.GetFullPath("import"), fileName + ".fms"));
    }
}

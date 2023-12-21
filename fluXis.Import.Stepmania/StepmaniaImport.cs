using System;
using System.IO;
using System.Linq;
using fluXis.Game.Import;
using fluXis.Game.Overlay.Notifications;
using fluXis.Import.Stepmania.Map;
using JetBrains.Annotations;
using Newtonsoft.Json;
using osu.Framework.Logging;

namespace fluXis.Import.Stepmania;

[UsedImplicitly]
public class StepmaniaImport : MapImporter
{
    public override string[] FileExtensions => new[] { ".sm" };
    public override string GameName => "Stepmania";
    public override string Color => "#f0d01f";

    public override void Import(string path)
    {
        var notification = CreateNotification();

        try
        {
            var smFolder = Path.GetDirectoryName(path);
            var fileName = Path.GetFileNameWithoutExtension(path);
            var folder = CreateTempFolder(fileName);

            Directory.GetFiles(smFolder).ToList().ForEach(x =>
            {
                if (x.EndsWith(".sm"))
                {
                    var data = File.ReadAllText(x);

                    var map = new StepmaniaFile();
                    map.Parse(data);

                    var infos = map.ToMapInfos();

                    foreach (var info in infos)
                    {
                        var json = JsonConvert.SerializeObject(info);
                        WriteFile(json, folder, $"{info.Metadata.Difficulty}.fsc");
                    }
                }
                else
                    CopyFile(x, folder);
            });

            var pack = CreatePackage(fileName, folder);
            FinalizeConversion(pack, notification);
            CleanUp(folder);
        }
        catch (Exception e)
        {
            notification.State = LoadingState.Failed;
            Logger.Error(e, "Error while importing Stepmania map");
        }
    }
}

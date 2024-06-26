using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using fluXis.Game.Database.Maps;
using fluXis.Game.Import;
using fluXis.Game.Overlay.Notifications;
using fluXis.Import.Stepmania.AutoImport;
using fluXis.Import.Stepmania.Map;
using fluXis.Shared.Utils;
using JetBrains.Annotations;
using osu.Framework.Bindables;
using osu.Framework.Logging;

namespace fluXis.Import.Stepmania;

[UsedImplicitly]
public class StepmaniaImport : MapImporter
{
    public override string[] FileExtensions => new[] { ".sm" };
    public override string GameName => "Stepmania";
    public override string Color => "#f0d01f";
    public override bool SupportsAutoImport => true;

    private Bindable<string> path { get; }

    public StepmaniaImport(StepmaniaPluginConfig config)
    {
        path = config.GetBindable<string>(StepmaniaPluginSetting.GameLocation);
    }

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
                        WriteFile(info.Serialize(), folder, $"{info.Metadata.Difficulty}.fsc");
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

    public override List<RealmMapSet> GetMaps()
    {
        var sets = new List<RealmMapSet>();

        if (string.IsNullOrEmpty(path.Value) || !Directory.Exists(path.Value))
            return sets;

        var resources = GetResourceProvider(path.Value);
        var files = Directory.GetFiles(path.Value, "*.sm", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            var data = File.ReadAllText(file);

            var sm = new StepmaniaFile();
            sm.Parse(data);

            var infos = sm.ToMapInfos();
            var maps = new List<RealmMap>();

            foreach (var info in infos)
            {
                var hits = info.HitObjects.Count(x => !x.LongNote);
                var lns = info.HitObjects.Count(x => x.LongNote);
                var length = info.HitObjects.Max(x => x.Time);

                var map = new StepManiaRealmMap
                {
                    Difficulty = info.Metadata.Difficulty,
                    FileName = Path.GetFileName(file),
                    KeyCount = 4,
                    StatusInt = ID,
                    MapInfo = info,
                    Metadata = new RealmMapMetadata
                    {
                        Title = info.Metadata.Title,
                        Artist = info.Metadata.Artist,
                        Source = info.Metadata.Source,
                        Audio = info.AudioFile,
                        Background = info.BackgroundFile,
                        PreviewTime = info.Metadata.PreviewTime,
                        Tags = info.Metadata.Tags
                    },
                    Filters = new RealmMapFilters
                    {
                        Length = (float)length,
                        BPMMin = info.TimingPoints.Min(x => x.BPM),
                        BPMMax = info.TimingPoints.Max(x => x.BPM),
                        NoteCount = hits,
                        LongNoteCount = lns,
                        NotesPerSecond = (float)((hits + lns) / (length / 1000f))
                    }
                };

                maps.Add(map);
            }

            var set = new StepmaniaRealmMapSet(maps)
            {
                FolderPath = Path.GetDirectoryName(file),
                ID = Guid.NewGuid(),
                Resources = resources,
                OnlineID = -1
            };

            sets.Add(set);
        }

        return sets;
    }
}

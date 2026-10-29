using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using fluXis.Database.Maps;
using fluXis.Import.Stepmania.AutoImport;
using fluXis.Import.Stepmania.Map;
using fluXis.Map.Format;
using fluXis.Map.Structures;
using fluXis.Modes.Keys.Objects;
using fluXis.Overlay.Notifications;
using JetBrains.Annotations;
using osu.Framework.Bindables;
using osu.Framework.Logging;
using osu.Framework.Platform;

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
        if (!File.Exists(path))
            return;

        var notification = CreateNotification();

        try
        {
            var smFolder = Path.GetDirectoryName(path);
            var fileName = Path.GetFileNameWithoutExtension(path);
            var folder = CreateTempFolder(fileName);

            var storage = new NativeStorage(folder);
            var format = new RhymMapFormat(storage, GameModes);

            Directory.GetFiles(smFolder).ToList().ForEach(x =>
            {
                if (x.EndsWith(".sm"))
                {
                    var data = File.ReadAllText(x);

                    var map = new StepmaniaFile();
                    map.Parse(data);

                    var infos = map.ToPlayableMaps(storage);

                    foreach (var info in infos)
                        format.Save(info);
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

            Storage storage = new NativeStorage(Path.GetDirectoryName(file));
            var infos = sm.ToPlayableMaps(storage);
            var maps = new List<RealmMap>();

            foreach (var info in infos)
            {
                var hits = info.Objects.Count(x => x is Note);
                var lns = info.Objects.Count(x => x is LongNote);
                var mines = info.Objects.Count(x => x is Landmine);
                var length = info.Objects.Max(x => x.Time);
                var keyCount = info.ObjectsOfType<HitObject>().Max(x => x.Lane);

                var map = new StepManiaRealmMap
                {
                    Difficulty = info.Difficulty,
                    FileName = Path.GetFileName(file),
                    KeyCount = keyCount,
                    StatusInt = ID,
                    PlayableMap = info,
                    Metadata = new RealmMapMetadata
                    {
                        Title = info.Title,
                        Artist = info.Artist,
                        Source = info.Sources.GetValueOrDefault("audio", ""),
                        Audio = info.AudioFile,
                        Background = info.BackgroundFile,
                        PreviewTime = info.PreviewTime,
                        Tags = string.Join(",", info.Tags)
                    },
                    Filters = new RealmMapFilters
                    {
                        Length = (float)length,
                        BPMMin = info.ObjectsOfType<TimingPoint>().Min(x => x.BPM),
                        BPMMax = info.ObjectsOfType<TimingPoint>().Max(x => x.BPM),
                        NoteCount = hits,
                        LongNoteCount = lns,
                        LandmineCount = mines,
                        NotesPerSecond = (float)((hits + lns) / (length / 1000f))
                    }
                };

                // info.RealmEntry = map;
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

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using fluXis.Game.Database.Maps;
using fluXis.Game.Import;
using fluXis.Game.Overlay.Notifications;
using fluXis.Import.osu.Map;
using fluXis.Import.osu.Map.Enums;
using fluXis.Game.Overlay.Notifications.Types.Loading;
using fluXis.Import.osu.AutoImport;
using JetBrains.Annotations;
using Newtonsoft.Json;
using osu_database_reader.BinaryFiles;
using osu.Framework.Logging;
using osu.Shared;

namespace fluXis.Import.osu;

[UsedImplicitly]
public class OsuImport : MapImporter
{
    public override string[] FileExtensions => new[] { ".osz" };
    public override string Name => "osu!mania";
    public override string Author => "Flustix";
    public override Version Version => new(1, 1, 0);
    public override bool SupportsAutoImport => true;
    public override string Color => "#e7659f";
    public override string StoragePath { get; } = string.Empty;

    public OsuImport()
    {
        StoragePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "osu!", "Songs");
    }

    public override void Import(string path)
    {
        var notification = new LoadingNotificationData
        {
            TextLoading = "Importing osu! map...",
            TextSuccess = "Imported osu! map!",
            TextFailure = "Failed to import osu! map!"
        };

        Notifications.Add(notification);

        try
        {
            Logger.Log("Importing osu! map: " + path);

            string folder = Path.GetFileNameWithoutExtension(path);

            ZipArchive osz = ZipFile.OpenRead(path);

            var success = 0;
            var failed = 0;

            foreach (var entry in osz.Entries)
            {
                if (entry.FullName.EndsWith(".osu"))
                {
                    try
                    {
                        OsuMap map = parseOsuMap(entry);
                        string json = JsonConvert.SerializeObject(map.ToMapInfo());
                        WriteFile(json, folder + "/" + entry.FullName + ".fsc");

                        notification.TextSuccess = $"Imported osu! map: {map.Artist} - {map.Title}";
                        success++;
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e, "Error while importing osu! map");
                        failed++;
                    }
                }
                else
                    CopyFile(entry, folder);
            }

            osz.Dispose();

            if (success == 0)
            {
                if (failed == 0)
                    notification.TextFailure = "No osu!mania maps found in the .osz file";

                notification.State = LoadingState.Failed;
                return;
            }

            if (failed > 0)
                notification.TextSuccess += $" ({failed} failed)";

            ZipArchive fms = ZipFile.Open(Path.Combine(Storage.GetFullPath("import"), folder + ".fms"), ZipArchiveMode.Create);

            // add all files from the import folder
            foreach (var file in Directory.GetFiles(Path.Combine(Storage.GetFullPath("import"), folder)))
                fms.CreateEntryFromFile(file, Path.GetFileName(file));

            fms.Dispose();
            Directory.Delete(Path.Combine(Storage.GetFullPath("import"), folder), true);

            var import = new FluXisImport
            {
                MapStatus = ID,
                Notification = notification,
                Realm = Realm,
                MapStore = MapStore,
                Storage = Storage,
                Notifications = Notifications
            };
            import.Import(Path.Combine(Storage.GetFullPath("import"), folder + ".fms"));
        }
        catch (Exception e)
        {
            notification.State = LoadingState.Failed;
            Logger.Error(e, "Error while importing osu! map");
        }
    }

    private OsuMap parseOsuMap(ZipArchiveEntry entry) => ParseOsuMap(new StreamReader(entry.Open()).ReadToEnd());

    public static OsuMap ParseOsuMap(string fileContent)
    {
        string[] lines = fileContent.Split(Environment.NewLine);

        OsuParser parser = new();
        OsuFileSection section = OsuFileSection.General;

        foreach (var line in lines)
        {
            if (line.StartsWith("["))
            {
                section = sectionFromString(line);
                continue;
            }

            // ignore comment
            if (line.StartsWith("//")) continue;

            // ignore empty lines
            if (string.IsNullOrEmpty(line)) continue;

            // ignore file version
            if (line.StartsWith("osu file format")) continue;

            parser.AddLine(line, section);
        }

        return parser.Parse();
    }

    private static OsuFileSection sectionFromString(string line)
    {
        string section = line.Substring(1, line.Length - 2);

        return section switch
        {
            "General" => OsuFileSection.General,
            "Editor" => OsuFileSection.Editor,
            "Metadata" => OsuFileSection.Metadata,
            "Difficulty" => OsuFileSection.Difficulty,
            "Events" => OsuFileSection.Events,
            "TimingPoints" => OsuFileSection.TimingPoints,
            "Colours" => OsuFileSection.Colours,
            "HitObjects" => OsuFileSection.HitObjects,
            _ => OsuFileSection.General
        };
    }

    public override List<RealmMapSet> GetMaps()
    {
        if (!File.Exists(StoragePath + "/../osu!.db"))
            return new List<RealmMapSet>();

        var db = OsuDb.Read(StoragePath + "/../osu!.db");

        var maps = db.Beatmaps.Where(b => b.GameMode == GameMode.Mania);
        var sets = maps.GroupBy(b => b.FolderName);

        var mapSets = new List<RealmMapSet>();

        foreach (var set in sets)
        {
            try
            {
                var mapList = new List<RealmMap>();

                var realmMapSet = new OsuRealmMapSet(mapList)
                {
                    Managed = true,
                    Resources = Resources
                };

                foreach (var map in set)
                {
                    // an extremely stupid thing we need to do...
                    // to get the map background, we have to load the map,
                    // because for some reason the background is not in the .db file
                    // this increades the loading time by a lot, but there is no other way
                    var osuMap = ParseOsuMap(File.ReadAllText(StoragePath + "/" + map.FolderName + "/" + map.BeatmapFileName));

                    var realmMap = new OsuRealmMap
                    {
                        OsuPath = StoragePath,
                        FolderPath = map.FolderName,
                        Difficulty = map.Version,
                        Metadata = new RealmMapMetadata
                        {
                            Title = map.Title,
                            Artist = map.Artist,
                            Mapper = map.Creator,
                            Source = map.SongSource,
                            Tags = map.SongTags,
                            Background = osuMap.GetBackground(),
                            Audio = map.AudioFileName,
                            PreviewTime = map.AudioPreviewTime
                        },
                        MapSet = realmMapSet,
                        Status = ID,
                        FileName = map.BeatmapFileName,
                        OnlineID = 0,
                        Hash = null,
                        Filters = new RealmMapFilters
                        {
                            Length = map.TotalTime,
                            BPMMin = 0,
                            BPMMax = 0,
                            NoteCount = map.CountHitCircles,
                            LongNoteCount = map.CountSliders,
                            NotesPerSecond = (map.CountHitCircles + map.CountSliders) / (float)map.TotalTime,
                            HasScrollVelocity = false,
                            HasLaneSwitch = false,
                            HasFlash = false
                        },
                        KeyCount = (int)map.CircleSize,
                        Rating = 0
                    };

                    if (map.TimingPoints != null)
                    {
                        try
                        {
                            realmMap.Filters.BPMMax = realmMap.Filters.BPMMin = (float)Math.Round(60000 / map.TimingPoints.Find(x => x.MsPerQuarter > 0).MsPerQuarter, 0);
                        }
                        catch (Exception)
                        {
                            realmMap.Filters.BPMMax = realmMap.Filters.BPMMin = 0;
                        }
                    }

                    mapList.Add(realmMap);
                }

                mapSets.Add(realmMapSet);
            }
            catch (Exception e)
            {
                Logger.Error(e, "Error while importing osu! map");
            }
        }

        return mapSets;
    }
}

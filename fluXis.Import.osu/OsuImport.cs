using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using fluXis.Database.Maps;
using fluXis.Import.osu.Map;
using fluXis.Import.osu.Map.Enums;
using fluXis.Import.osu.AutoImport;
using fluXis.Import.osu.Storyboards;
using fluXis.Overlay.Notifications;
using fluXis.Utils;
using JetBrains.Annotations;
using osu_database_reader.BinaryFiles;
using osu.Framework.Bindables;
using osu.Framework.Logging;
using osu.Shared;

namespace fluXis.Import.osu;

[UsedImplicitly]
public class OsuImport : MapImporter
{
    public override string[] FileExtensions => new[] { ".osz" };
    public override string GameName => "osu!mania";
    public override bool SupportsAutoImport => true;
    public override string Color => "#e7659f";

    private Bindable<string> osuPath { get; }
    private Bindable<bool> skipBackgrounds { get; }

    public OsuImport(OsuPluginConfig config)
    {
        osuPath = config.GetBindable<string>(OsuPluginSetting.GameLocation);
        skipBackgrounds = config.GetBindable<bool>(OsuPluginSetting.SkipBackgrounds);
    }

    public override void Import(string path)
    {
        if (!File.Exists(path))
            return;

        var notification = CreateNotification();

        try
        {
            var fileName = Path.GetFileNameWithoutExtension(path);
            var folder = CreateTempFolder(fileName);

            using var osz = ZipFile.OpenRead(path);

            var sb = osz.Entries.FirstOrDefault(e => e.FullName.EndsWith(".osb"))?.FullName;

            if (sb != null)
            {
                var entry = osz.GetEntry(sb);

                if (entry != null)
                {
                    var data = new StreamReader(entry.Open()).ReadToEnd();
                    var storyboard = new OsuStoryboardParser().Parse(data);
                    var json = storyboard.Serialize();
                    WriteFile(json, folder, $"{sb}.fsb");
                    sb = $"{sb}.fsb";
                }
                else
                {
                    Logger.Log("Failed to find storyboard file in .osz archive!", LoggingTarget.Runtime, LogLevel.Error);
                    sb = string.Empty;
                }
            }

            var success = 0;
            var failed = 0;

            foreach (var entry in osz.Entries)
            {
                if (entry.FullName.EndsWith(".osu"))
                {
                    try
                    {
                        var map = parseOsuMap(entry);
                        var info = map.ToMapInfo();

                        if (info == null)
                        {
                            failed++;
                            continue;
                        }

                        info.StoryboardFile = sb ?? string.Empty;
                        WriteFile(info.Serialize(), folder, $"{entry.FullName}.fsc");
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

            if (success == 0)
            {
                if (failed == 0)
                    notification.TextFailed = "No osu!mania maps found in the .osz archive!";

                notification.State = LoadingState.Failed;
                return;
            }

            if (failed > 0)
                notification.TextFinished += $" ({failed} failed)";

            var pack = CreatePackage(fileName, folder);
            FinalizeConversion(pack, notification);
            CleanUp(folder);
        }
        catch (Exception e)
        {
            notification.State = LoadingState.Failed;
            Logger.Error(e, "Error while importing osu! map.");
        }
    }

    private OsuMap parseOsuMap(ZipArchiveEntry entry) => ParseOsuMap(new StreamReader(entry.Open()).ReadToEnd());

    public static OsuMap ParseOsuMap(string fileContent, bool eventsOnly = false)
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

        return parser.Parse(eventsOnly);
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
        if (!File.Exists(osuPath.Value + "/osu!.db"))
            return new List<RealmMapSet>();

        var db = OsuDb.Read(osuPath.Value + "/osu!.db");

        var songsPath = Path.Combine(osuPath.Value, "Songs");
        var resources = GetResourceProvider(songsPath);

        var maps = db.Beatmaps.Where(b => b.GameMode == GameMode.Mania);
        var sets = maps.GroupBy(b => b.FolderName).ToArray();

        var mapSets = new List<RealmMapSet>();

        foreach (var set in sets)
        {
            try
            {
                var mapList = new List<RealmMap>();

                var realmMapSet = new OsuRealmMapSet(mapList)
                {
                    FolderPath = Path.Combine(songsPath, set.Key),
                    Resources = resources
                };

                foreach (var map in set)
                {
                    var realmMap = new OsuRealmMap
                    {
                        Difficulty = map.Version,
                        MapSet = realmMapSet,
                        StatusInt = ID,
                        FileName = map.BeatmapFileName,
                        HealthDifficulty = map.HPDrainRate,
                        AccuracyDifficulty = map.OveralDifficulty,
                        KeyCount = (int)map.CircleSize,
                        Hash = map.BeatmapChecksum,
                        OnlineID = -1,
                        Metadata = new RealmMapMetadata
                        {
                            Title = map.Title,
                            Artist = map.Artist,
                            Mapper = map.Creator,
                            Source = map.SongSource,
                            Tags = map.SongTags,
                            Audio = map.AudioFileName,
                            PreviewTime = map.AudioPreviewTime
                        },
                        Filters = new RealmMapFilters
                        {
                            Length = map.TotalTime,
                            NoteCount = map.CountHitCircles,
                            LongNoteCount = map.CountSliders,
                            NotesPerSecond = (map.CountHitCircles + map.CountSliders) / (map.TotalTime / 1000f)
                        }
                    };

                    if (!skipBackgrounds.Value)
                    {
                        // an extremely stupid thing we need to do...
                        // to get the map background, we have to load the map,
                        // because for some reason the background is not in the .db file
                        // this increases the loading time by a lot, but there is no other way
                        var mapPath = Path.Combine(songsPath, map.FolderName, map.BeatmapFileName);

                        if (!File.Exists(mapPath))
                            continue;

                        var osuMap = ParseOsuMap(File.ReadAllText(mapPath), true);
                        realmMap.Metadata.Background = osuMap.GetBackground();
                    }

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

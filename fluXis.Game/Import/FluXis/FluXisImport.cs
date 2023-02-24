using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using fluXis.Game.Database;
using fluXis.Game.Database.Maps;
using fluXis.Game.Map;
using Newtonsoft.Json;
using osu.Framework.Logging;
using osu.Framework.Platform;

namespace fluXis.Game.Import.FluXis;

public class FluXisImport : MapImporter
{
    /**
     * Used to set the status of the next imported mapset.
     */
    public int MapStatus { get; set; } = -2;

    public FluXisImport(FluXisRealm realm, MapStore mapStore, Storage storage)
        : base(realm, mapStore, storage)
    {
    }

    public override Task Import(string path)
    {
        return new Task(() =>
        {
            Logger.Log($"Loading mapset from {path}");

            ZipArchive archive = ZipFile.OpenRead(path);

            List<RealmFile> files = new();
            List<RealmMap> maps = new();

            RealmMapSet mapSet = new(maps, files);

            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                string hash = GetHash(entry);

                string filename = entry.FullName;

                files.Add(new RealmFile
                {
                    Hash = hash,
                    Name = entry.FullName,
                });

                if (filename.EndsWith(".fsc"))
                {
                    string json = new StreamReader(entry.Open()).ReadToEnd();
                    MapInfo mapInfo = JsonConvert.DeserializeObject<MapInfo>(json);

                    float length = 0;
                    int keys = 0;
                    float bpmMin = float.MaxValue;
                    float bpmMax = float.MinValue;

                    foreach (var point in mapInfo.TimingPoints)
                    {
                        bpmMin = Math.Min(bpmMin, point.BPM);
                        bpmMax = Math.Max(bpmMax, point.BPM);
                    }

                    foreach (var hitObject in mapInfo.HitObjects)
                    {
                        float time = hitObject.Time;
                        if (hitObject.IsLongNote()) time += hitObject.HoldTime;
                        length = Math.Max(length, time);

                        keys = Math.Max(keys, hitObject.Lane);
                    }

                    RealmMap map = new RealmMap(new RealmMapMetadata
                    {
                        Title = mapInfo.Metadata.Title ?? "Untitled",
                        Artist = mapInfo.Metadata.Artist ?? "Unknown",
                        Mapper = mapInfo.Metadata.Mapper ?? "Unknown",
                        Source = mapInfo.Metadata.Source ?? "",
                        Tags = mapInfo.Metadata.Tags ?? "",
                        Audio = mapInfo.AudioFile,
                        Background = mapInfo.BackgroundFile,
                        PreviewTime = mapInfo.Metadata.PreviewTime,
                    })
                    {
                        Difficulty = mapInfo.Metadata.Difficulty ?? "Unknown",
                        MapSet = mapSet,
                        Hash = hash,
                        KeyCount = keys,
                        Length = length,
                        BPMMin = bpmMin,
                        BPMMax = bpmMax,
                        Rating = 0,
                        Status = MapStatus
                    };

                    maps.Add(map);
                }
            }

            if (files.Count > 0 && maps.Count > 0)
            {
                Realm.RunWrite(realm =>
                {
                    realm.Add(mapSet);
                    MapStore.AddMapSet(mapSet.Detach());

                    foreach (var file in files)
                    {
                        string filePath = Storage.GetStorageForDirectory("files").GetFullPath(file.GetPath());
                        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                        if (File.Exists(filePath)) continue;

                        ZipArchiveEntry entry = archive.GetEntry(file.Name);
                        entry.ExtractToFile(filePath);
                    }

                    archive.Dispose();

                    try { File.Delete(path); }
                    catch { Logger.Log($"Failed to delete {path}"); }
                });
            }
        });
    }
}

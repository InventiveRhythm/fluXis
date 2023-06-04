using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.IO.Compression;
using System.Linq;
using fluXis.Game.Database.Maps;
using fluXis.Game.Import;
using fluXis.Game.Map;
using fluXis.Game.Overlay.Notification;
using fluXis.Import.Quaver.Map;
using Newtonsoft.Json;
using osu.Framework.Logging;
using YamlDotNet.Serialization;

namespace fluXis.Import.Quaver;

public class QuaverImport : MapImporter
{
    public override string[] FileExtensions => new[] { ".qp" };
    public override string Name => "Quaver";
    public override string Color => "#0cb2d8";
    public override string StoragePath => quaverPath;

    private string quaverPath = "";

    public override void Import(string path)
    {
        var notification = new LoadingNotification
        {
            TextLoading = "Importing Quaver map...",
            TextSuccess = "Imported Quaver map!",
            TextFailure = "Failed to import Quaver map!"
        };

        Notifications.AddNotification(notification);

        try
        {
            Logger.Log("Importing Quaver map: " + path);
            string fileName = Path.GetFileNameWithoutExtension(path);

            ZipArchive qp = ZipFile.OpenRead(path);

            List<MapInfo> mapInfos = new();

            foreach (var entry in qp.Entries)
            {
                if (entry.FullName.EndsWith(".qua"))
                {
                    QuaverMap quaverMap = parseQuaverMap(entry);
                    MapInfo map = quaverMap.ToMapInfo();
                    mapInfos.Add(map);

                    notification.TextSuccess = $"Imported Quaver map: {map.Metadata.Artist} - {map.Metadata.Title}";

                    string effect = quaverMap.GetEffects();
                    Logger.Log(effect);

                    if (effect != "")
                    {
                        string name = entry.FullName.ToLower() + ".ffx";
                        string dest = Path.Combine(Storage.GetFullPath("import"), fileName, name);
                        Directory.CreateDirectory(Path.GetDirectoryName(dest));
                        File.WriteAllText(dest, effect);

                        map.EffectFile = name;
                    }

                    string json = JsonConvert.SerializeObject(map);
                    string destPath = Path.Combine(Storage.GetFullPath("import"), fileName, entry.FullName + ".fsc");
                    Directory.CreateDirectory(Path.GetDirectoryName(destPath));
                    File.WriteAllText(destPath, json);
                }
                else CopyFile(entry, fileName);
            }

            qp.Dispose();

            ZipArchive fms = ZipFile.Open(Path.Combine(Storage.GetFullPath("import"), fileName + ".fms"), ZipArchiveMode.Create);

            // add all files from the import folder
            foreach (var file in Directory.GetFiles(Path.Combine(Storage.GetFullPath("import"), fileName)))
                fms.CreateEntryFromFile(file, Path.GetFileName(file));

            fms.Dispose();
            Directory.Delete(Path.Combine(Storage.GetFullPath("import"), fileName), true);

            var import = new FluXisImport
            {
                MapStatus = ID,
                Notification = notification,
                Realm = Realm,
                MapStore = MapStore,
                Storage = Storage,
                Notifications = Notifications
            };
            import.Import(Path.Combine(Storage.GetFullPath("import"), fileName + ".fms"));
        }
        catch (Exception e)
        {
            notification.State = LoadingState.Failed;
            Logger.Error(e, "Error while importing Quaver map");
        }
    }

    private QuaverMap parseQuaverMap(ZipArchiveEntry entry)
    {
        string yaml = new StreamReader(entry.Open()).ReadToEnd();
        return parseFromYaml(yaml);
    }

    private QuaverMap parseFromYaml(string yaml)
    {
        var builder = new DeserializerBuilder();
        builder.IgnoreUnmatchedProperties();
        var deserializer = builder.Build();

        QuaverMap map = deserializer.Deserialize<QuaverMap>(yaml);
        return map;
    }

    public override List<RealmMapSet> GetMaps()
    {
        const string c_path = @"C:\Program Files (x86)\Steam\steamapps\common\Quaver\Quaver.exe";
        var installPath = "";

        if (File.Exists(c_path)) installPath = c_path;
        else
        {
            string[] drives = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
                              .Select(c => $@"{c}:\")
                              .Where(Directory.Exists)
                              .ToArray();

            const string drive_path = @"SteamLibrary\steamapps\common\Quaver\Quaver.exe";

            foreach (var drive in drives)
            {
                if (File.Exists($@"{drive}{drive_path}"))
                {
                    installPath = $@"{drive}{drive_path}";
                    break;
                }
            }
        }

        if (string.IsNullOrEmpty(installPath))
        {
            Logger.Log("Could not find Quaver install");
            return base.GetMaps();
        }

        Logger.Log($"Found Quaver install at {installPath}");

        string directory = Path.GetDirectoryName(installPath);
        string dbPath = Path.Combine(directory, "quaver.db");
        quaverPath = directory + "\\" + "Songs";

        if (!File.Exists(dbPath))
        {
            Logger.Log("Could not find Quaver database");
            return base.GetMaps();
        }

        try
        {
            SQLiteConnection connection = new SQLiteConnection($"Data Source={dbPath};Version=3;");
            connection.Open();

            SQLiteCommand command = new SQLiteCommand("SELECT * FROM Map", connection);
            SQLiteDataReader reader = command.ExecuteReader();

            Dictionary<string, List<RealmMap>> maps = new();

            while (reader.Read())
            {
                string directoryName = reader["Directory"].ToString();
                string path = reader["Path"].ToString();
                int mapsetId = int.Parse(reader["MapSetId"].ToString());
                string artist = reader["Artist"].ToString();
                string title = reader["Title"].ToString();
                string difficulty = reader["DifficultyName"].ToString();
                string creator = reader["Creator"].ToString();
                string background = reader["BackgroundPath"].ToString();
                string audio = reader["AudioPath"].ToString();
                int previewTime = int.Parse(reader["AudioPreviewTime"].ToString());
                string source = reader["Source"].ToString();
                string tags = reader["Tags"].ToString();
                float bpm = float.Parse(reader["BPM"].ToString());
                float songLength = float.Parse(reader["SongLength"].ToString());
                int mode = int.Parse(reader["Mode"].ToString());
                int noteCount = int.Parse(reader["RegularNoteCount"].ToString());
                int longNoteCount = int.Parse(reader["LongNoteCount"].ToString());

                var map = new RealmMap
                {
                    ID = default,
                    Hash = path, // we'll use the path as the hash
                    Difficulty = difficulty,
                    Metadata = new RealmMapMetadata
                    {
                        Title = title,
                        Artist = artist,
                        Mapper = creator,
                        Source = source,
                        Tags = tags,
                        Background = background,
                        Audio = audio,
                        PreviewTime = previewTime
                    },
                    Status = ID,
                    OnlineID = 0,
                    Filters = new RealmMapFilters
                    {
                        Length = songLength,
                        BPMMin = bpm,
                        BPMMax = bpm,
                        NoteCount = noteCount,
                        LongNoteCount = longNoteCount,
                        NotesPerSecond = 0,
                        HasScrollVelocity = false,
                        HasLaneSwitch = false,
                        HasFlash = false
                    },
                    KeyCount = mode == 1 ? 4 : 7,
                    Rating = 0
                };

                if (!maps.ContainsKey(directoryName)) maps.Add(directoryName, new List<RealmMap>());
                maps[directoryName].Add(map);
            }

            reader.Close();

            Logger.Log(JsonConvert.SerializeObject(maps));

            foreach (var mapSet in maps)
            {
                var mapSetMaps = mapSet.Value;

                var mapSetRealm = new RealmMapSet(mapSetMaps)
                {
                    ID = default,
                    OnlineID = 0,
                    Cover = "",
                    Managed = true,
                    Path = mapSet.Key
                };

                foreach (var map in mapSetMaps)
                {
                    map.MapSet = mapSetRealm;
                }

                MapStore.AddMapSet(mapSetRealm);
            }
        }
        catch (Exception e)
        {
            Logger.Error(e, "Error while reading Quaver database");
            return base.GetMaps();
        }

        return base.GetMaps();
    }

    public override string GetAsset(RealmMap map, ImportedAssetType type)
    {
        string directory = map.MapSet.Path;
        string path = "";

        switch (type)
        {
            case ImportedAssetType.Background or ImportedAssetType.Cover:
                path = map.Metadata.Background;
                break;

            case ImportedAssetType.Audio:
                path = map.Metadata.Audio;
                break;
        }

        return Path.Combine(directory, path);
    }

    public override MapPackage GetMapPackage(RealmMap map)
    {
        var path = Path.Combine(quaverPath, map.MapSet.Path);
        path = Path.Combine(path, map.Hash);

        string yaml = File.ReadAllText(path);
        var quaverMap = parseFromYaml(yaml);

        var package = new MapPackage
        {
            MapInfo = quaverMap.ToMapInfo(),
            MapEvents = new MapEvents().Load(quaverMap.GetEffects()),
        };

        return package;
    }
}

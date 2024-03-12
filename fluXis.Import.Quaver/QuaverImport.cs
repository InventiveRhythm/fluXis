using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.IO.Compression;
using fluXis.Game.Database.Maps;
using fluXis.Game.Import;
using fluXis.Game.Overlay.Notifications;
using fluXis.Import.Quaver.Map;
using fluXis.Shared.Utils;
using JetBrains.Annotations;
using osu.Framework.Bindables;
using osu.Framework.Logging;
using YamlDotNet.Serialization;

namespace fluXis.Import.Quaver;

[UsedImplicitly]
public class QuaverImport : MapImporter
{
    public override string[] FileExtensions => new[] { ".qp" };
    public override string GameName => "Quaver";
    public override bool SupportsAutoImport => true;
    public override string Color => "#0cb2d8";

    private Bindable<string> quaverPath { get; }
    private string songsPath => string.IsNullOrEmpty(quaverPath.Value) ? "" : Path.Combine(quaverPath.Value, "Songs");

    public QuaverImport(QuaverPluginConfig config)
    {
        quaverPath = config.GetBindable<string>(QuaverPluginSetting.GameLocation);
    }

    public override void Import(string path)
    {
        var notification = CreateNotification();

        try
        {
            var fileName = Path.GetFileNameWithoutExtension(path);
            var folder = CreateTempFolder(fileName);

            var qp = ZipFile.OpenRead(path);

            foreach (var entry in qp.Entries)
            {
                if (entry.FullName.EndsWith(".qua"))
                {
                    var quaverMap = parseFromEntry(entry);
                    var map = quaverMap.ToMapInfo();

                    var effect = quaverMap.GetEffects().Save();

                    if (effect != "")
                    {
                        var name = $"{entry.FullName.ToLower()}.ffx";
                        map.EffectFile = name;
                        WriteFile(effect, folder, name);
                    }

                    var json = map.Serialize();
                    WriteFile(json, folder, $"{entry.FullName}.fsc");
                }
                else
                    CopyFile(entry, folder);
            }

            qp.Dispose();

            var pack = CreatePackage(fileName, folder);
            FinalizeConversion(pack, notification);
            CleanUp(folder);
        }
        catch (Exception e)
        {
            notification.State = LoadingState.Failed;
            Logger.Error(e, "Error while importing Quaver map");
        }
    }

    private static QuaverMap parseFromEntry(ZipArchiveEntry entry)
    {
        string yaml = new StreamReader(entry.Open()).ReadToEnd();
        return ParseFromYaml(yaml);
    }

    public static QuaverMap ParseFromYaml(string yaml)
    {
        var builder = new DeserializerBuilder();
        builder.IgnoreUnmatchedProperties();
        var deserializer = builder.Build();

        QuaverMap map = deserializer.Deserialize<QuaverMap>(yaml);
        return map;
    }

    public override List<RealmMapSet> GetMaps()
    {
        if (string.IsNullOrEmpty(quaverPath.Value))
            return base.GetMaps();

        string dbPath = Path.Combine(quaverPath.Value, "quaver.db");

        if (!File.Exists(dbPath))
        {
            Logger.Log($"Could not find Quaver database at {dbPath}");
            return base.GetMaps();
        }

        List<RealmMapSet> mapSets = new();

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

                var map = new QuaverRealmMap
                {
                    FileName = path,
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
                    StatusInt = ID,
                    OnlineID = 0,
                    Filters = new RealmMapFilters
                    {
                        Length = songLength,
                        BPMMin = bpm,
                        BPMMax = bpm,
                        NoteCount = noteCount,
                        LongNoteCount = longNoteCount,
                        NotesPerSecond = (noteCount + longNoteCount) / (songLength / 1000),
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

            var resources = GetResourceProvider(songsPath);

            foreach (var (directoryName, mapSetMaps) in maps)
            {
                var mapSetRealm = new QuaverRealmMapSet(mapSetMaps)
                {
                    FolderPath = Path.Combine(songsPath, directoryName),
                    OnlineID = 0,
                    Cover = "",
                    Managed = true,
                    Resources = resources
                };

                foreach (var map in mapSetMaps)
                    map.MapSet = mapSetRealm;

                mapSets.Add(mapSetRealm);
            }
        }
        catch (Exception e)
        {
            Logger.Error(e, "Error while reading Quaver database");
            return base.GetMaps();
        }

        return mapSets;
    }
}

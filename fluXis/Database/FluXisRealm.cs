using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using fluXis.Database.Input;
using fluXis.Database.Maps;
using fluXis.Database.Score;
using fluXis.Map;
using fluXis.Utils;
using osu.Framework.Development;
using osu.Framework.Logging;
using osu.Framework.Platform;
using Realms;
using Realms.Dynamic;

namespace fluXis.Database;

public class FluXisRealm : IDisposable
{
    /// <summary>
    /// 2 - fixed a bug where multiple keybinds could be created for the same action
    /// 3 - add mods to RealmScore
    /// 4 - add Cover to RealmMapSet
    /// 5 - add RealmMapFilters
    /// 6 - add AutoImport to ImporterInfo
    /// 7 - Make RealmScore.Mods a string
    /// 8 - Added PlayerID to RealmScore
    /// 9 - Removed RealmJudgements and moved to RealmScore
    /// 10 - Changed storage system
    /// 11 - Added ColorHex to RealmMapMetadata
    /// 12 - Added ID to RealmKeybind
    /// 13 - Added RealmMapSet.DateAdded, RealmMapSet.DateSubmitted, RealmMapSet.DateRanked,
    ///    - RealmMap.LastLocalUpdate, RealmMap.LastOnlineUpdate, RealmMap.LastPlayed,
    ///    - RealmMap.OnlineHash, RealmMap.AccuracyDifficulty and RealmMap.HealthDifficulty
    /// 14 - Replaced RealmMapFilters effects with a bitfield
    /// 15 - Add `PerformanceRating` and `ScrollSpeed` to `RealmScore` and change OnlineID to long
    /// 16 - Add `Version` to `RealmScore`
    /// 17 - Romanizable Title and Artist
    /// 18 - Reset online score IDs
    /// </summary>
    private const int schema_version = 18;

    private Realm updateRealm;

    private Realm realm
    {
        get
        {
            if (!ThreadSafety.IsUpdateThread)
                throw new InvalidOperationException("Realm can only be accessed from the update thread.");

            return updateRealm ??= getInstance();
        }
    }

    private readonly Storage storage;

    private RealmConfiguration config => new(storage.GetFullPath("fluxis.realm"))
    {
        SchemaVersion = schema_version,
        MigrationCallback = onMigrate
    };

    private void onMigrate(Migration migration, ulong oldSchemaVersion)
    {
        for (ulong i = oldSchemaVersion + 1; i <= schema_version; i++)
            migrateTo(migration, i);
    }

    private void migrateTo(Migration migration, ulong targetSchemaVersion)
    {
        switch (targetSchemaVersion)
        {
            case 2:
                Dictionary<string, RealmKeybind> keys = new Dictionary<string, RealmKeybind>();
                List<RealmKeybind> toRemove = new List<RealmKeybind>();

                foreach (var keybind in migration.NewRealm.All<RealmKeybind>())
                {
                    if (!keys.TryAdd(keybind.Action, keybind))
                    {
                        var existing = keys[keybind.Action];
                        toRemove.Add(existing);
                        keys[keybind.Action] = keybind;
                    }
                }

                foreach (var keybind in toRemove) migration.NewRealm.Remove(keybind);
                break;

            case 3 or 4:
                break; // nothing to do here

            case 5:
                var newMaps = migration.NewRealm.All<RealmMap>().ToList();

                foreach (var newMap in newMaps)
                {
                    MapInfo map = newMap.GetMapInfo();
                    if (map == null) continue;

                    MapEvents events = new MapEvents();

                    if (!string.IsNullOrEmpty(map.EffectFile))
                    {
                        dynamic effectFile = newMap.MapSet.GetPathForFile(map.EffectFile);
                        string effectPath = storage.GetFullPath("files/" + PathUtils.HashToPath(effectFile.Hash));
                        string content = File.ReadAllText(effectPath);
                        events = MapEvents.Load<MapEvents>(content);
                    }

                    newMap.Filters = MapUtils.GetMapFilters(map, events);
                }

                break;

            case 7:
                var newScores = migration.NewRealm.All<RealmScore>().ToList();

                foreach (var score in newScores)
                    score.Mods = string.Empty;

                break;

            case 8:
                var newScores2 = migration.NewRealm.All<RealmScore>().ToList();

                foreach (var score in newScores2)
                    score.PlayerID = -1;

                break;

            case 9:
                var oldScores = migration.OldRealm.DynamicApi.All("RealmScore").ToList();
                var newScores3 = migration.NewRealm.All<RealmScore>().ToList();

                // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
                foreach (DynamicRealmObject oldScore in oldScores)
                {
                    var judgement = oldScore.DynamicApi.Get<dynamic>("Judgements");
                    var newScore = newScores3.FirstOrDefault(s => s.ID == oldScore.DynamicApi.Get<Guid>("ID"));

                    if (newScore == null) continue;

                    newScore.Flawless = (int)judgement.Flawless;
                    newScore.Perfect = (int)judgement.Perfect;
                    newScore.Great = (int)judgement.Great;
                    newScore.Alright = (int)judgement.Alright;
                    newScore.Okay = (int)judgement.Okay;
                    newScore.Miss = (int)judgement.Miss;
                }

                break;

            case 10:
                var mapsets = migration.OldRealm.DynamicApi.All("RealmMapSet").ToList();
                var newMapsets = migration.NewRealm.All<RealmMapSet>().ToList();

                foreach (var mapset in mapsets)
                {
                    var guid = mapset.DynamicApi.Get<Guid>("ID");
                    var newMapset = newMapsets.FirstOrDefault(m => m.ID == guid);
                    if (newMapset == null) throw new Exception("Failed to find mapset");

                    string folder = guid.ToString();
                    var maps = (RealmList<dynamic>)mapset.DynamicApi.GetList<dynamic>("Maps");
                    var files = (RealmList<dynamic>)mapset.DynamicApi.GetList<dynamic>("Files");

                    foreach (var map in maps)
                    {
                        var id = map.DynamicApi.Get<Guid>("ID");
                        var newMap = newMapset.Maps.FirstOrDefault(m => m.ID == id);
                        if (newMap == null) throw new Exception("Failed to find map");

                        var file = files.FirstOrDefault(f => f.DynamicApi.Get<string>("Hash") == newMap.Hash);
                        if (file == null) throw new Exception("Failed to find file");

                        newMap.FileName = file.DynamicApi.Get<string>("Name");
                    }

                    var fullFolder = storage.GetFullPath("maps/" + folder) + "/";
                    Logger.Log($"Migrating mapset {folder} to {fullFolder}");

                    foreach (var file in files)
                    {
                        var name = file.Name;
                        var hash = file.Hash;

                        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(hash)) continue;

                        var path = PathUtils.HashToPath(hash);
                        var fullPath = storage.GetFullPath("files/" + path);

                        if (!File.Exists(fullPath)) continue;

                        Directory.CreateDirectory(Path.GetDirectoryName(fullFolder + name));
                        File.Copy(fullPath, fullFolder + name, true);
                    }
                }

                Directory.Delete(storage.GetFullPath("files"), true);

                break;

            case 12:
                var newKeyBindings = migration.NewRealm.All<RealmKeybind>().ToList();

                foreach (var keybind in newKeyBindings)
                {
                    keybind.ID = Guid.NewGuid();
                }

                break;

            case 13:
            {
                var sets = migration.NewRealm.All<RealmMapSet>().ToList();

                foreach (var set in sets)
                {
                    set.DateAdded = DateTimeOffset.Now;

                    foreach (var map in set.Maps)
                    {
                        map.LastLocalUpdate = DateTimeOffset.Now;

                        var info = map.GetMapInfo();
                        map.AccuracyDifficulty = info?.AccuracyDifficulty ?? 8;
                    }
                }

                break;
            }

            case 14:
            {
                var maps = migration.NewRealm.All<RealmMap>().ToList();

                foreach (var filter in migration.NewRealm.All<RealmMapFilters>().ToList())
                    migration.NewRealm.Remove(filter);

                foreach (var map in maps)
                {
                    var info = map.GetMapInfo();
                    var events = info?.GetMapEvents();

                    var filters = MapUtils.GetMapFilters(info, events);
                    map.Filters = filters;
                }

                break;
            }

            case 16:
            {
                var scores = migration.NewRealm.All<RealmScore>().ToList();
                scores.ForEach(s => s.Version = 1);
                break;
            }

            case 17:
            {
                var maps = migration.NewRealm.All<RealmMap>().ToList();
                maps.ForEach(m =>
                {
                    if (m.Metadata is null)
                        return;

                    m.Metadata.TitleRomanized = m.Metadata.Title;
                    m.Metadata.ArtistRomanized = m.Metadata.Artist;
                });
                break;
            }

            case 18:
            {
                var scores = migration.NewRealm.All<RealmScore>().ToList();
                scores.ForEach(x => x.OnlineID = -1);
                break;
            }
        }
    }

    public FluXisRealm(Storage storage)
    {
        this.storage = storage;
    }

    private Realm getInstance()
    {
        return Realm.GetInstance(config);
    }

    public T Run<T>(Func<Realm, T> action)
    {
        if (ThreadSafety.IsUpdateThread)
            return action(realm);

        using var r = getInstance();
        return action(r);
    }

    public void Run(Action<Realm> action)
    {
        if (ThreadSafety.IsUpdateThread)
            action(realm);
        else
        {
            using var r = getInstance();
            action(r);
        }
    }

    public T RunWrite<T>(Func<Realm, T> action)
    {
        if (ThreadSafety.IsUpdateThread)
            return write(realm, action);

        using var r = getInstance();
        return write(r, action);
    }

    public void RunWrite(Action<Realm> action)
    {
        if (ThreadSafety.IsUpdateThread)
            write(realm, action);
        else
        {
            using var r = getInstance();
            write(r, action);
        }
    }

    private static T write<T>(Realm realm, Func<Realm, T> func)
    {
        Transaction transaction = null;

        try
        {
            if (!realm.IsInTransaction)
                transaction = realm.BeginWrite();

            var result = func(realm);
            transaction?.Commit();
            return result;
        }
        finally
        {
            transaction?.Dispose();
        }
    }

    private static void write(Realm realm, Action<Realm> func)
    {
        Transaction transaction = null;

        try
        {
            if (!realm.IsInTransaction)
                transaction = realm.BeginWrite();

            func(realm);
            transaction?.Commit();
        }
        finally
        {
            transaction?.Dispose();
        }
    }

    public void Dispose()
    {
        updateRealm?.Dispose();
        GC.SuppressFinalize(this);
    }
}

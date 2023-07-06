using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using fluXis.Game.Database.Input;
using fluXis.Game.Database.Maps;
using fluXis.Game.Database.Score;
using fluXis.Game.Map;
using fluXis.Game.Utils;
using osu.Framework.Development;
using osu.Framework.Logging;
using osu.Framework.Platform;
using Realms;

namespace fluXis.Game.Database;

public class FluXisRealm : IDisposable
{
    /// <summary>
    /// 2 - fixed a bug where multiple keybinds could be created for the same action
    /// 3 - add mods to RealmScore
    /// 4 - add Cover to RealmMapSet
    /// 5 - add RealmMapFilters
    /// 6 - add AutoImport to ImporterInfo
    /// 7 - Make RealmScore.Mods a string
    /// </summary>
    private const int schema_version = 7;

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
                    if (keys.ContainsKey(keybind.Action))
                    {
                        var existing = keys[keybind.Action];
                        toRemove.Add(existing);
                        keys[keybind.Action] = keybind;
                    }
                    else keys.Add(keybind.Action, keybind);
                }

                foreach (var keybind in toRemove) migration.NewRealm.Remove(keybind);
                break;

            case 3 or 4:
                break; // nothing to do here

            case 5:
                var newMaps = migration.NewRealm.All<RealmMap>().ToList();

                foreach (var newMap in newMaps)
                {
                    string path = storage.GetFullPath("files/" + PathUtils.HashToPath(newMap.Hash));

                    MapInfo map = MapUtils.LoadFromPath(path);

                    if (map == null)
                    {
                        Logger.Log($"[RealmMigration 4 -> 5] Map file not found: {path}??? Something definitely went wrong here.");
                        continue;
                    }

                    MapEvents events = new MapEvents();

                    if (!string.IsNullOrEmpty(map.EffectFile))
                    {
                        RealmFile effectFile = newMap.MapSet.GetFile(map.EffectFile);
                        string effectPath = storage.GetFullPath("files/" + PathUtils.HashToPath(effectFile.Hash));
                        string content = File.ReadAllText(effectPath);
                        events.Load(content);
                    }

                    newMap.Filters = MapUtils.GetMapFilters(map, events);
                }

                break;

            case 7:
                var newScores = migration.NewRealm.All<RealmScore>().ToList();

                foreach (var score in newScores)
                    score.Mods = string.Empty;

                break;
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

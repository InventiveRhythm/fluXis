using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using fluXis.Game.Database;
using fluXis.Game.Database.Maps;
using fluXis.Game.Map;
using fluXis.Game.Overlay.Notification;
using JetBrains.Annotations;
using osu.Framework;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Textures;
using osu.Framework.IO.Stores;
using osu.Framework.Logging;
using osu.Framework.Platform;
using Realms;

namespace fluXis.Game.Import;

public partial class ImportManager : Component
{
    private const string lib_prefix = "fluXis.Import";

    [Resolved]
    private FluXisRealm realm { get; set; }

    [Resolved]
    private MapStore mapStore { get; set; }

    [Resolved]
    private Storage storage { get; set; }

    [Resolved]
    private NotificationOverlay notifications { get; set; }

    [Resolved]
    private AudioManager audio { get; set; }

    private List<ImportPlugin> plugins { get; } = new();
    public IEnumerable<ImportPlugin> Plugins => plugins.ToImmutableArray();

    private List<MapImporter> importers { get; } = new();
    private Dictionary<int, Storage> storages { get; } = new();
    private Dictionary<int, ITextureStore> textureStores { get; } = new();
    private Dictionary<int, ITrackStore> trackStores { get; } = new();

    [BackgroundDependencyLoader]
    private void load(GameHost host)
    {
        loadFromAppDomain();
        loadFromRunFolder();
        loadFromPlugins();

        foreach (var importer in importers)
        {
            var shouldImport = realm.Run(r => r.All<ImporterInfo>().FirstOrDefault(i => i.Id == importer.ID)?.AutoImport ?? false);
            if (!shouldImport || !importer.SupportsAutoImport) continue;

            var maps = importer.GetMaps();
            foreach (var map in maps) mapStore.AddMapSet(map);

            if (!string.IsNullOrEmpty(importer.StoragePath))
            {
                var storageFor = new NativeStorage(importer.StoragePath);
                storages.Add(importer.ID, storageFor);

                var resourceStore = new StorageBackedResourceStore(storageFor);
                textureStores.Add(importer.ID, new LargeTextureStore(host.Renderer, host.CreateTextureLoaderStore(resourceStore)));
                trackStores.Add(importer.ID, audio.GetTrackStore(resourceStore));
            }
        }
    }

    [CanBeNull]
    public Storage GetStorage(int id)
    {
        if (storages.TryGetValue(id, out var s)) return s;

        return null;
    }

    [CanBeNull]
    public ITextureStore GetTextureStore(int id)
    {
        if (textureStores.TryGetValue(id, out var s)) return s;

        return null;
    }

    [CanBeNull]
    public ITrackStore GetTrackStore(int id)
    {
        if (trackStores.TryGetValue(id, out var s)) return s;

        return null;
    }

    [CanBeNull]
    public MapPackage GetMapPackage(RealmMap map)
    {
        var importer = importers.FirstOrDefault(i => i.ID == map.Status);
        return importer?.GetMapPackage(map);
    }

    public void Import(string path)
    {
        try
        {
            var importer = importers.FirstOrDefault(i => i.FileExtensions.Contains(Path.GetExtension(path)));

            if (importer == null)
            {
                Logger.Log($"No importer found for {path}");
                return;
            }

            importer.Import(path);
        }
        catch (Exception e)
        {
            Logger.Error(e, "Error while importing mapset");
            notifications.PostError("Error while importing mapset");
        }
    }

    public void ImportMultiple(string[] paths)
    {
        foreach (var path in paths) Import(path);
    }

    public string GetAsset(RealmMap map, ImportedAssetType type)
    {
        var importer = importers.FirstOrDefault(i => i.ID == map.Status);

        if (importer == null) return "";

        return importer.GetAsset(map, type);
    }

    private void loadSingle(Assembly assembly)
    {
        string name = assembly.GetName().Name;

        try
        {
            var types = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(MapImporter)));

            foreach (var type in types)
            {
                var importer = (MapImporter)Activator.CreateInstance(type);

                if (importer == null)
                {
                    Logger.Log($"Failed to load importer {name}");
                    continue;
                }

                Logger.Log($"Loaded importer {importer.GetType().Name} ({string.Join(", ", importer.FileExtensions)})");

                if (importer is not FluXisImport)
                {
                    realm.RunWrite(r =>
                    {
                        var existing = r.All<ImporterInfo>().FirstOrDefault(i => i.Name == importer.Name);

                        if (existing == null)
                        {
                            var id = getNewId(r);

                            r.Add(new ImporterInfo
                            {
                                Name = importer.Name,
                                Color = importer.Color,
                                Id = id
                            });

                            importer.ID = id;
                            Logger.Log($"Assigned id {id} to importer {importer.Name}");
                        }
                        else
                        {
                            existing.Color = existing.Color;
                            importer.ID = existing.Id;
                            Logger.Log($"Importer {importer.Name} has id {existing.Id}");
                        }
                    });

                    plugins.Add(new ImportPlugin
                    {
                        Name = importer.Name,
                        Author = importer.Author,
                        Version = importer.Version,
                        HasAutoImport = importer.SupportsAutoImport,
                        ImporterId = importer.ID
                    });
                }

                importer.Realm = realm;
                importer.MapStore = mapStore;
                importer.Storage = storage;
                importer.Notifications = notifications;
                importers.Add(importer);
            }
        }
        catch (Exception e)
        {
            Logger.Error(e, $"Failed to load importer {name}");
        }
    }

    private void loadFromAppDomain()
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            var name = assembly.GetName().Name;

            if (name == null)
                continue;

            if (!name.StartsWith("fluXis", StringComparison.InvariantCultureIgnoreCase))
                continue;

            loadSingle(assembly);
        }
    }

    private void loadFromRunFolder()
    {
        string[] files = Directory.GetFiles(RuntimeInfo.StartupDirectory, $"{lib_prefix}.*.dll");

        foreach (var file in files)
        {
            try
            {
                var assembly = Assembly.LoadFrom(file);
                loadSingle(assembly);
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Failed to load importer {file} from directory from AppDomain!");
            }
        }
    }

    private void loadFromPlugins()
    {
        var plugins = storage.GetFullPath("plugins");

        if (!Directory.Exists(plugins))
        {
            Logger.Log($"Plugins directory {plugins} does not exist. Creating...");
            Directory.CreateDirectory(plugins);
        }

        string[] files = Directory.GetFiles(plugins, $"{lib_prefix}.*.dll");

        foreach (var file in files)
        {
            try
            {
                var assembly = Assembly.LoadFrom(file);
                loadSingle(assembly);
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Failed to load importer {file} from plugins!");
            }
        }
    }

    private int getNewId(Realm realm)
    {
        var highest = 99;

        foreach (var importer in realm.All<ImporterInfo>())
        {
            if (importer.Id > highest)
                highest = importer.Id;
        }

        return highest + 1;
    }
}

public enum ImportedAssetType
{
    Background,
    Cover,
    Audio
}

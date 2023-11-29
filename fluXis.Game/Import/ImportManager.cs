using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using fluXis.Game.Database;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Graphics.Background.Cropped;
using fluXis.Game.Map;
using fluXis.Game.Overlay.Notifications;
using osu.Framework;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Graphics;
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
    private NotificationManager notifications { get; set; }

    [Resolved]
    private AudioManager audio { get; set; }

    [Resolved]
    private GameHost host { get; set; }

    [Resolved]
    private FluXisGameBase game { get; set; }

    private List<ImportPlugin> plugins { get; } = new();
    public IEnumerable<ImportPlugin> Plugins => plugins.ToImmutableArray();

    private Dictionary<ImportPlugin, MapImporter> importersByPlugin { get; } = new();
    private Dictionary<MapImporter, List<RealmMapSet>> importedMaps { get; } = new();

    private List<MapImporter> importers { get; } = new();

    public List<Task> TaskQueue { get; } = new();

    [BackgroundDependencyLoader]
    private void load()
    {
        loadFromAppDomain();
        loadFromRunFolder();
        loadFromPlugins();

        Logger.Log($"Loaded {importers.Count} importers");

        Schedule(() =>
        {
            setToolbarText("Checking for auto-imports...");

            foreach (var importer in importers)
                ImportMapsFrom(importer);

            setToolbarText("");
        });
    }

    protected override void Update()
    {
        base.Update();

        if (TaskQueue.Count <= 0) return;

        var task = TaskQueue[0];

        if (task.Status == TaskStatus.Created)
            task.Start();

        if (task.IsCompleted)
            TaskQueue.RemoveAt(0);
    }

    private void setToolbarText(string text)
    {
        Schedule(() =>
        {
            if (string.IsNullOrEmpty(text))
            {
                game.Toolbar.CenterText.FadeOut(200);
                return;
            }

            game.Toolbar.CenterText.Text = text;
            game.Toolbar.CenterText.FadeIn(200);
        });
    }

    public MapImporter GetImporter(ImportPlugin plugin) => importersByPlugin[plugin];

    public void ImportMapsFrom(MapImporter importer)
    {
        TaskQueue.Add(new Task(() =>
        {
            var shouldImport = realm.Run(r => r.All<ImporterInfo>().FirstOrDefault(i => i.Id == importer.ID)?.AutoImport ?? false);
            if (!shouldImport || !importer.SupportsAutoImport) return;

            Logger.Log($"Auto-importing {importer.Name} maps...");
            long time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            setToolbarText($"Importing {importer.Name} maps...");

            var maps = importer.GetMaps();

            foreach (var map in maps)
            {
                mapStore.AddMapSet(map, false);

                if (!importedMaps.ContainsKey(importer))
                    importedMaps.Add(importer, new List<RealmMapSet>());

                importedMaps[importer].Add(map);
            }

            mapStore.CollectionUpdated?.Invoke();
            Logger.Log($"Imported {maps.Count} {importer.Name} maps in {DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - time}ms");
            setToolbarText("");
        }));
    }

    public void RemoveImportedMaps(MapImporter importer)
    {
        if (!importedMaps.ContainsKey(importer))
            return;

        foreach (var map in importedMaps[importer])
            mapStore.Remove(map);

        importedMaps.Remove(importer);
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
            notifications.SendError("Error while importing mapset", e.Message);
        }
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

                    var plugin = new ImportPlugin
                    {
                        Name = importer.Name,
                        Author = importer.Author,
                        Version = importer.Version,
                        HasAutoImport = importer.SupportsAutoImport,
                        ImporterId = importer.ID
                    };

                    plugins.Add(plugin);
                    importersByPlugin.Add(plugin, importer);

                    if (!string.IsNullOrEmpty(importer.StoragePath))
                    {
                        var storageFor = new NativeStorage(importer.StoragePath);

                        importer.Resources = new MapResourceProvider
                        {
                            TrackStore = audio.GetTrackStore(new StorageBackedResourceStore(storageFor)),
                            BackgroundStore = new BackgroundTextureStore(host, storageFor),
                            CroppedBackgroundStore = new CroppedBackgroundStore(host, storageFor)
                        };
                    }
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
        var path = storage.GetFullPath("plugins");

        if (!Directory.Exists(path))
        {
            Logger.Log($"Plugins directory {path} does not exist. Creating...");
            Directory.CreateDirectory(path);
        }

        string[] files = Directory.GetFiles(path, $"{lib_prefix}.*.dll");

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

    private static int getNewId(Realm realm)
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

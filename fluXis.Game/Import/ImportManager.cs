using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using fluXis.Game.Database;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Graphics.Background.Cropped;
using fluXis.Game.Map;
using fluXis.Game.Overlay.Notifications;
using fluXis.Game.Overlay.Toolbar;
using fluXis.Game.Plugins;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Extensions.IEnumerableExtensions;
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

    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private Toolbar toolbar { get; set; }

    private Dictionary<Plugin, MapImporter> importersByPlugin { get; } = new();
    private Dictionary<MapImporter, List<RealmMapSet>> importedMaps { get; } = new();
    private List<MapImporter> importers { get; } = new();
    private List<Task> taskQueue { get; } = new();

    [BackgroundDependencyLoader]
    private void load(PluginManager plugins)
    {
        foreach (var plugin in plugins.Plugins)
        {
            var importer = plugin.Importer;
            if (importer == null) continue;

            realm.RunWrite(r =>
            {
                var existing = r.All<ImporterInfo>().FirstOrDefault(i => i.Name == importer.GameName);

                if (existing == null)
                {
                    var id = getNewId(r);

                    r.Add(new ImporterInfo
                    {
                        Name = importer.GameName,
                        Color = importer.Color,
                        Id = id
                    });

                    importer.ID = id;
                    Logger.Log($"Assigned id {id} to importer {importer.GameName}");
                }
                else
                {
                    existing.Color = existing.Color;
                    importer.ID = existing.Id;
                    Logger.Log($"Importer {importer.GameName} has id {existing.Id}");
                }
            });

            importersByPlugin.Add(plugin, importer);

            importer.ResourceRequest = path =>
            {
                var storageFor = new NativeStorage(path);
                var resourceStore = new StorageBackedResourceStore(storageFor);

                var resource = new MapResourceProvider
                {
                    TrackStore = audio.GetTrackStore(resourceStore),
                    SampleStore = audio.GetSampleStore(resourceStore),
                    BackgroundStore = new BackgroundTextureStore(host, storageFor),
                    CroppedBackgroundStore = new CroppedBackgroundStore(host, storageFor)
                };

                return resource;
            };

            importer.Realm = realm;
            importer.MapStore = mapStore;
            importer.Storage = storage;
            importer.Notifications = notifications;
            importers.Add(importer);
        }

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

        if (taskQueue.Count <= 0) return;

        var task = taskQueue[0];

        if (task.Status == TaskStatus.Created)
            task.Start();

        if (task.IsCompleted)
            taskQueue.RemoveAt(0);
    }

    private void setToolbarText(string text)
    {
        if (toolbar == null)
            return;

        Schedule(() => toolbar.SetCenterText(text));
    }

    public MapImporter GetImporter(Plugin plugin) => importersByPlugin[plugin];

    public void ImportMapsFrom(MapImporter importer)
    {
        taskQueue.Add(new Task(() =>
        {
            var shouldImport = realm.Run(r => r.All<ImporterInfo>().FirstOrDefault(i => i.Id == importer.ID)?.AutoImport ?? false);
            if (!shouldImport || !importer.SupportsAutoImport) return;

            Logger.Log($"Auto-importing {importer.GameName} maps...");
            long time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            setToolbarText($"Importing {importer.GameName} maps...");

            var sets = importer.GetMaps();

            foreach (var set in sets)
            {
                set.AutoImported = true;
                set.OnlineID = -1;
                set.Maps.ForEach(m => m.OnlineID = -1);

                mapStore.AddMapSet(set, false);

                if (!importedMaps.ContainsKey(importer))
                    importedMaps.Add(importer, new List<RealmMapSet>());

                importedMaps[importer].Add(set);
            }

            mapStore.CollectionUpdated?.Invoke();
            Logger.Log($"Imported {sets.Count} {importer.GameName} maps in {DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - time}ms");
            setToolbarText("");
        }));
    }

    public void RemoveImportedMaps(MapImporter importer)
    {
        if (!importedMaps.TryGetValue(importer, out var maps))
            return;

        foreach (var map in maps)
            mapStore.Remove(map);

        importedMaps.Remove(importer);
    }

    public void Import(string path)
    {
        var extension = Path.GetExtension(path);

        if (extension == ".fms")
        {
            new FluXisImport
            {
                Realm = realm,
                MapStore = mapStore,
                Storage = storage,
                Notifications = notifications
            }.Import(path);
            return;
        }

        try
        {
            var importer = importers.FirstOrDefault(i => i.FileExtensions.Contains(extension));

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

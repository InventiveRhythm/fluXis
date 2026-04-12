using System.IO;
using System.Linq;
using System.Threading.Tasks;
using fluXis.Database;
using fluXis.Database.Maps;
using fluXis.Database.Score;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Localization;
using fluXis.Localization.Categories.Settings;
using fluXis.Map;
using fluXis.Overlay.Notifications;
using fluXis.Overlay.Notifications.Tasks;
using fluXis.Overlay.Settings.UI;
using fluXis.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Framework.Platform;

namespace fluXis.Overlay.Settings.Sections.Advanced;

public partial class AdvancedMapsSection : SettingsSubSection
{
    public override LocalisableString Title => strings.Maps;
    public override IconUsage Icon => FontAwesome6.Solid.Map;

    private SettingsAdvancedStrings strings => LocalizationStrings.Settings.Advanced;

    private bool isRunning;

    [Resolved]
    private MapStore mapStore { get; set; }

    [Resolved]
    private FluXisRealm realm { get; set; }

    [Resolved]
    private NotificationManager notifications { get; set; }

    [Resolved]
    private GameHost host { get; set; }

    private Storage cacheStorage => host.CacheStorage.GetStorageForDirectory(FluXisGame.FFT_CACHE_PATH);

    private SettingsItem clearFFTOption;

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRange(new Drawable[]
        {
            new SettingsButton
            {
                Label = strings.RecalculateFilters,
                Description = strings.RecalculateFiltersDescription,
                ButtonText = "Run",
                Action = recalculateFilters
            },
            new SettingsButton
            {
                Label = strings.CleanUpScores,
                Description = strings.CleanUpScoresDescription,
                ButtonText = "Run",
                Action = clearScores
            },
            clearFFTOption = new SettingsButton
            {
                Label = strings.ClearVisualizationCache + " (???KB)",
                Description = strings.ClearVisualizationCacheDescription,
                ButtonText = "Run",
                Action = clearFFTCache
            }
        });

        Task.Run(() =>
        {
            var path = cacheStorage.GetFullPath(FluXisGame.FFT_CACHE_PATH);
            var dir = new DirectoryInfo(path);

            if (!dir.Exists)
            {
                var labelNoCache = $"{strings.ClearVisualizationCache} (No Cache)";
                Schedule(() => clearFFTOption.SetLabel(labelNoCache));

                return;
            }

            var bytes = dir.GetFiles("*", SearchOption.AllDirectories).Sum(f => f.Length);

            var label = $"{strings.ClearVisualizationCache} ({StringUtils.FormatBytes(bytes)})";
            Schedule(() => clearFFTOption.SetLabel(label));
        });
    }

    private void recalculateFilters()
    {
        if (isRunning)
            return;

        isRunning = true;

        var notification = new TaskNotificationData
        {
            Text = "Recalculating Filters..."
        };

        notifications.AddTask(notification);

        Task.Run(() =>
        {
            realm.RunWrite(r =>
            {
                var count = r.All<RealmMap>().Count();
                var idx = 0;

                foreach (var filters in r.All<RealmMapFilters>())
                    r.Remove(filters);

                foreach (var set in mapStore.MapSets)
                {
                    foreach (var map in set.Maps)
                    {
                        notification.Progress = (float)idx++ / count;

                        var existing = r.Find<RealmMap>(map.ID);

                        if (existing == null)
                            continue;

                        var data = map.GetMapInfo();

                        if (data is null)
                            continue;

                        existing.AccuracyDifficulty = map.AccuracyDifficulty = data.AccuracyDifficulty;
                        existing.HealthDifficulty = map.HealthDifficulty = data.HealthDifficulty;

                        var events = data.GetMapEvents();

                        var filters = MapUtils.GetMapFilters(data, events);
                        existing.Filters = filters;
                        map.Filters = filters.Detach();
                    }
                }
            });

            notification.State = LoadingState.Complete;
            isRunning = false;
        });
    }

    private void clearScores()
    {
        var count = 0;

        realm.RunWrite(r =>
        {
            var scores = r.All<RealmScore>().ToList();
            var maps = r.All<RealmMap>().ToList();

            foreach (var score in scores)
            {
                if (maps.Any(m => m.ID == score.MapID))
                    continue;

                count++;
                r.Remove(score);
            }
        });

        notifications.SendSmallText($"Removed {count} scores.", FontAwesome6.Solid.Check);
    }

    private void clearFFTCache()
    {
        cacheStorage.DeleteDirectory(FluXisGame.FFT_CACHE_PATH);
        clearFFTOption.SetLabel($"{strings.ClearVisualizationCache} (No Cache)");
    }
}

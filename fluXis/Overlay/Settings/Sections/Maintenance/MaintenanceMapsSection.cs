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

namespace fluXis.Overlay.Settings.Sections.Maintenance;

public partial class MaintenanceMapsSection : SettingsSubSection
{
    public override LocalisableString Title => strings.Maps;
    public override IconUsage Icon => FontAwesome6.Solid.Map;

    private SettingsMaintenanceStrings strings => LocalizationStrings.Settings.Maintenance;

    private bool isRunning;

    [Resolved]
    private MapStore mapStore { get; set; }

    [Resolved]
    private FluXisRealm realm { get; set; }

    [Resolved]
    private NotificationManager notifications { get; set; }

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
                Label = strings.UpdateAllMaps,
                Description = strings.UpdateAllMapsDescription,
                ButtonText = "Run",
                Action = mapStore.UpdateAllMaps
            },
            new SettingsButton
            {
                Label = strings.CleanUpScores,
                Description = strings.CleanUpScoresDescription,
                ButtonText = "Run",
                Action = clearScores
            }
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
}

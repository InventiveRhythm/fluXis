using System.Linq;
using System.Threading.Tasks;
using fluXis.Game.Database;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Localization;
using fluXis.Game.Localization.Categories.Settings;
using fluXis.Game.Map;
using fluXis.Game.Overlay.Notifications;
using fluXis.Game.Overlay.Notifications.Tasks;
using fluXis.Game.Overlay.Settings.UI;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace fluXis.Game.Overlay.Settings.Sections.Maintenance;

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
}

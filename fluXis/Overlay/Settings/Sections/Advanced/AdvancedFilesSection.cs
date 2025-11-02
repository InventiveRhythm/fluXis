using System;
using System.IO;
using System.Linq;
using fluXis.Configuration;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Localization;
using fluXis.Localization.Categories.Settings;
using fluXis.Map;
using fluXis.Overlay.Notifications;
using fluXis.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Framework.Logging;
using osu.Framework.Platform;

namespace fluXis.Overlay.Settings.Sections.Advanced;

public partial class AdvancedFilesSection : SettingsSubSection
{
    public override LocalisableString Title => strings.Files;
    public override IconUsage Icon => FontAwesome6.Solid.File;

    private SettingsAdvancedStrings strings => LocalizationStrings.Settings.Advanced;

    [BackgroundDependencyLoader]
    private void load(Storage storage, MapStore store, NotificationManager notifications, FluXisConfig config)
    {
        AddRange(new Drawable[]
        {
            new SettingsButton
            {
                Label = strings.CleanUpFiles,
                Description = strings.CleanUpFilesDescription,
                ButtonText = "Run",
                Action = () =>
                {
                    notifications.SendSmallText("Cleaning up files...", FontAwesome6.Solid.Rotate);
                    var deleted = 0;
                    var errors = 0;

                    foreach (var directory in storage.GetDirectories("maps"))
                    {
                        var guid = directory.Split(Path.DirectorySeparatorChar).Last();

                        if (store.MapSets.All(m => m.ID.ToString() != guid))
                        {
                            try
                            {
                                storage.DeleteDirectory(directory);
                                deleted++;
                            }
                            catch (Exception ex)
                            {
                                errors++;
                                Logger.Error(ex, $"Failed to delete directory {directory}");
                            }
                        }
                    }

                    notifications.SendText($"Cleaned up {deleted} folder(s)", errors != 0 ? $"{errors} deletion(s) failed." : "", FontAwesome6.Solid.Check);
                }
            },
            new SettingsToggle
            {
                Label = strings.StreamFileBrowser,
                Description = strings.StreamFileBrowserDescription,
                Bindable = config.GetBindable<bool>(FluXisSetting.StreamFileBrowser)
            },
        });
    }
}

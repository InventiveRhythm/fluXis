using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Localization;
using fluXis.Game.Localization.Categories.Settings;
using fluXis.Game.Overlay.Settings.UI;
using fluXis.Game.Screens.Gameplay.HUD;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osu.Framework.Platform;

namespace fluXis.Game.Overlay.Settings.Sections.Appearance;

public partial class AppearanceLayoutSection : SettingsSubSection
{
    public override LocalisableString Title => strings.Layout;
    public override IconUsage Icon => FontAwesome6.Solid.LayerGroup;

    private SettingsAppearanceStrings strings => LocalizationStrings.Settings.Appearance;

    [BackgroundDependencyLoader]
    private void load(LayoutManager layouts, Storage storage)
    {
        SettingsDropdown<HUDLayout> layoutDropdown;

        AddRange(new Drawable[]
        {
            layoutDropdown = new SettingsDropdown<HUDLayout>
            {
                Label = strings.LayoutCurrent,
                Description = strings.LayoutCurrentDescription,
                Bindable = layouts.Layout,
                Items = layouts.Layouts
            },
            new SettingsButton
            {
                Label = strings.LayoutReload,
                Description = strings.LayoutReloadDescription,
                Enabled = true,
                ButtonText = "Reload",
                Action = layouts.Reload
            },
            new SettingsButton
            {
                Label = strings.LayoutCreate,
                Description = strings.LayoutCreateDescription,
                Enabled = true,
                ButtonText = "Create",
                Action = layouts.CreateNewLayout
            },
            new SettingsButton
            {
                Label = strings.LayoutShowInExplorer,
                Description = strings.LayoutShowInExplorerDescription,
                Enabled = true,
                ButtonText = "Show",
                Action = () =>
                {
                    if (layouts.Layout.Value is LayoutManager.DefaultLayout)
                        PathUtils.OpenFolder(storage.GetFullPath("layouts"));
                    else
                        PathUtils.ShowFile(storage.GetFullPath($"layouts/{layouts.Layout.Value.ID}.json"));
                }
            },
            new SettingsButton
            {
                Label = strings.LayoutOpenEditor,
                Description = strings.LayoutOpenEditorDescription,
                Enabled = false,
                ButtonText = "Open"
            }
        });

        layouts.Reloaded += () => layoutDropdown.Items = layouts.Layouts;
    }
}

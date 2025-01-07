using fluXis.Graphics.Sprites;
using fluXis.Localization;
using fluXis.Localization.Categories.Settings;
using fluXis.Overlay.Settings.UI;
using fluXis.Screens.Gameplay.HUD;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace fluXis.Overlay.Settings.Sections.Appearance;

public partial class AppearanceLayoutSection : SettingsSubSection
{
    public override LocalisableString Title => strings.Layout;
    public override IconUsage Icon => FontAwesome6.Solid.LayerGroup;

    private SettingsAppearanceStrings strings => LocalizationStrings.Settings.Appearance;

    [BackgroundDependencyLoader]
    private void load(LayoutManager layouts)
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
                Action = layouts.PresentExternally
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

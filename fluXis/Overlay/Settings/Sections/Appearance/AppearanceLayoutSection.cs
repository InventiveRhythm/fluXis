using fluXis.Graphics.Sprites;
using fluXis.Localization;
using fluXis.Localization.Categories.Settings;
using fluXis.Overlay.Settings.UI;
using fluXis.Screens.Gameplay.HUD;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace fluXis.Overlay.Settings.Sections.Appearance;

public partial class AppearanceLayoutSection : SettingsSubSection
{
    public override LocalisableString Title => strings.Layout;
    public override IconUsage Icon => FontAwesome6.Solid.LayerGroup;

    private SettingsAppearanceStrings strings => LocalizationStrings.Settings.Appearance;

    [Resolved]
    private LayoutManager layouts { get; set; }

    private SettingsDropdown<HUDLayout> currentDropdown;
    private BindableBool buttonsEnabled;

    [BackgroundDependencyLoader(true)]
    private void load([CanBeNull] FluXisGame game)
    {
        buttonsEnabled = new BindableBool(true) { Value = !layouts.IsDefault };

        AddRange(new Drawable[]
        {
            currentDropdown = new SettingsDropdown<HUDLayout>
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
                Action = () => layouts.CreateNewLayout()
            },
            new SettingsButton
            {
                Label = strings.LayoutOpenFolder,
                Description = strings.LayoutOpenFolderDescription,
                Enabled = true,
                ButtonText = "Show",
                Action = layouts.PresentExternally
            },
            new SettingsButton
            {
                Label = strings.LayoutOpenEditor,
                Description = strings.LayoutOpenEditorDescription,
                ButtonText = "Open",
                EnabledBindable = buttonsEnabled,
                Action = () => game?.OpenLayoutEditor()
            }
        });
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        layouts.Reloaded += () => currentDropdown.Items = layouts.Layouts;
        layouts.Layout.ValueChanged += _ => buttonsEnabled.Value = !layouts.IsDefault;
    }
}

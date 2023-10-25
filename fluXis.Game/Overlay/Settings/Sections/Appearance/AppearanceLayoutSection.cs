using fluXis.Game.Configuration;
using fluXis.Game.Overlay.Settings.UI;
using fluXis.Game.Screens.Gameplay.HUD;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Game.Overlay.Settings.Sections.Appearance;

public partial class AppearanceLayoutSection : SettingsSubSection
{
    public override string Title => "HUD Layout";

    [BackgroundDependencyLoader]
    private void load(LayoutManager layouts)
    {
        SettingsDropdown<HUDLayout> layoutDropdown;

        AddRange(new Drawable[]
        {
            layoutDropdown = new SettingsDropdown<HUDLayout>
            {
                Label = "Current Layout",
                Bindable = layouts.Layout,
                Items = layouts.Layouts
            },
            new SettingsButton
            {
                Label = "Reload Layouts",
                Enabled = true,
                ButtonText = "Reload",
                Action = layouts.Reload
            },
            new SettingsButton
            {
                Label = "Create New Layout",
                Enabled = true,
                ButtonText = "Create",
                Action = layouts.CreateNewLayout
            },
            new SettingsSlider<float>
            {
                Label = "Hit Error Bar Scale",
                Bindable = Config.GetBindable<float>(FluXisSetting.HitErrorScale),
                DisplayAsPercentage = true
            },
            new SettingsButton
            {
                Label = "Open Layout editor",
                Enabled = false,
                ButtonText = "Open"
            }
        });

        layouts.Reloaded += () => layoutDropdown.Items = layouts.Layouts;
    }
}

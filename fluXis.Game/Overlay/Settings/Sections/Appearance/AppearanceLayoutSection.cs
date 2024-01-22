using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Overlay.Settings.UI;
using fluXis.Game.Screens.Gameplay.HUD;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Platform;

namespace fluXis.Game.Overlay.Settings.Sections.Appearance;

public partial class AppearanceLayoutSection : SettingsSubSection
{
    public override string Title => "HUD Layout";
    public override IconUsage Icon => FontAwesome6.Solid.LayerGroup;

    [BackgroundDependencyLoader]
    private void load(LayoutManager layouts, Storage storage)
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
            new SettingsButton
            {
                Label = "Show layout file in explorer",
                Description = "Opens the folder containing the layout file in your file explorer. (This is temporary until the layout editor is finished)",
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
                Label = "Open Layout editor",
                Enabled = false,
                ButtonText = "Open"
            }
        });

        layouts.Reloaded += () => layoutDropdown.Items = layouts.Layouts;
    }
}

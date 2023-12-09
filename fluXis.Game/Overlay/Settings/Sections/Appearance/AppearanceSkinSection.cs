using fluXis.Game.Configuration;
using fluXis.Game.Overlay.Settings.UI;
using fluXis.Game.Skinning;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Overlay.Settings.Sections.Appearance;

public partial class AppearanceSkinSection : SettingsSubSection
{
    public override string Title => "Skin";
    public override IconUsage Icon => FontAwesome.Solid.PaintBrush;

    [BackgroundDependencyLoader]
    private void load(SkinManager skinManager, FluXisGameBase gameBase)
    {
        AddRange(new Drawable[]
        {
            new SettingsDropdown<string>
            {
                Label = "Current Skin",
                Bindable = Config.GetBindable<string>(FluXisSetting.SkinName),
                Items = skinManager.GetSkinNames()
            },
            new SettingsButton
            {
                Label = "Open Skin editor",
                ButtonText = "Open",
                Action = gameBase.OpenSkinEditor
            },
            new SettingsButton
            {
                Label = "Open Skin folder",
                Action = skinManager.OpenFolder,
                ButtonText = "Open"
            },
            new SettingsButton
            {
                Label = "Export Skin",
                Enabled = false,
                ButtonText = "Export"
            },
            new SettingsButton
            {
                Label = "Delete Skin",
                Enabled = false,
                ButtonText = "Delete"
            }
        });
    }
}

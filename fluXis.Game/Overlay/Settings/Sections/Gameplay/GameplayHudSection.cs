using System;
using fluXis.Game.Configuration;
using fluXis.Game.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Overlay.Settings.Sections.Gameplay;

public partial class GameplayHudSection : SettingsSubSection
{
    public override string Title => "HUD";
    public override IconUsage Icon => FontAwesome.Solid.LayerGroup;

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRange(new Drawable[]
        {
            new SettingsDropdown<HudVisibility>
            {
                Label = "Visibility",
                Items = Enum.GetValues<HudVisibility>(),
                Bindable = Config.GetBindable<HudVisibility>(FluXisSetting.HudVisibility)
            }
        });
    }
}

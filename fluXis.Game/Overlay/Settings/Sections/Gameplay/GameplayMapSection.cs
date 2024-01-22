using fluXis.Game.Configuration;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Overlay.Settings.Sections.Gameplay;

public partial class GameplayMapSection : SettingsSubSection
{
    public override string Title => "Map";
    public override IconUsage Icon => FontAwesome6.Solid.Map;

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRange(new Drawable[]
        {
            new SettingsToggle
            {
                Label = "Map Hitsounds",
                Description = "Use the map's custom hitsounds.",
                Bindable = Config.GetBindable<bool>(FluXisSetting.Hitsounding)
            },
            new SettingsToggle
            {
                Label = "Video / Storyboard",
                Description = "Show the video or storyboard in the background.",
                Bindable = Config.GetBindable<bool>(FluXisSetting.BackgroundVideo)
            },
        });
    }
}

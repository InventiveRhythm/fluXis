using fluXis.Game.Configuration;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Overlay.Settings.Sections.Gameplay;

public partial class GameplayScrollSpeedSection : SettingsSubSection
{
    public override string Title => "Scroll Speed";
    public override IconUsage Icon => FontAwesome6.Solid.UpDown;

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRange(new Drawable[]
        {
            new SettingsSlider<float>
            {
                Label = "Scroll Speed",
                Description = "Changes how fast the notes scroll.",
                Bindable = Config.GetBindable<float>(FluXisSetting.ScrollSpeed),
                Step = 0.1f
            }
        });
    }
}

using fluXis.Game.Configuration;
using fluXis.Game.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Game.Overlay.Settings.Sections.Audio;

public partial class AudioOffsetSection : SettingsSubSection
{
    public override string Title => "Offset";

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRange(new Drawable[]
        {
            new SettingsSlider<float>
            {
                Label = "Audio Offset",
                ValueLabel = "{value}ms",
                Bindable = Config.GetBindable<float>(FluXisSetting.GlobalOffset),
                Step = 1
            },
            new SettingsButton
            {
                Label = "Open Offset wizard",
                Enabled = false,
                ButtonText = "Open"
            },
        });
    }
}

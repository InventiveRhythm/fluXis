using fluXis.Configuration;
using fluXis.Graphics.Sprites;
using fluXis.Localization;
using fluXis.Localization.Categories.Settings;
using fluXis.Overlay.Settings.UI;
using fluXis.Screens;
using fluXis.Screens.Offset;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace fluXis.Overlay.Settings.Sections.Audio;

public partial class AudioOffsetSection : SettingsSubSection
{
    public override LocalisableString Title => strings.Offset;
    public override IconUsage Icon => FontAwesome6.Solid.Clock;

    private SettingsAudioStrings strings => LocalizationStrings.Settings.Audio;

    [BackgroundDependencyLoader(true)]
    private void load(FluXisScreenStack screens, SettingsMenu settings)
    {
        AddRange(new Drawable[]
        {
            new SettingsSlider<float>
            {
                Label = strings.AudioOffset,
                ValueLabel = "{value}ms",
                Description = strings.AudioOffsetDescription,
                Bindable = Config.GetBindable<float>(FluXisSetting.GlobalOffset),
                Step = 1
            },
            new SettingsToggle
            {
                Label = strings.DisableOffsetInReplay,
                Description = strings.DisableOffsetInReplayDescription,
                Bindable = Config.GetBindable<bool>(FluXisSetting.DisableOffsetInReplay)
            },
            new SettingsButton
            {
                Label = strings.OpenOffsetWizard,
                ButtonText = "Open",
                Action = () =>
                {
                    if (screens.CurrentScreen is OffsetSetup or not FluXisScreen { AllowExit: true })
                        return;

                    settings.Hide();
                    screens.Push(new OffsetSetup());
                }
            }
        });
    }
}

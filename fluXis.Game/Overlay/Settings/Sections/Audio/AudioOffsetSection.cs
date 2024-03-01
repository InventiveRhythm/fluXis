using fluXis.Game.Configuration;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Localization;
using fluXis.Game.Localization.Categories.Settings;
using fluXis.Game.Overlay.Settings.UI;
using fluXis.Game.Screens;
using fluXis.Game.Screens.Offset;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace fluXis.Game.Overlay.Settings.Sections.Audio;

public partial class AudioOffsetSection : SettingsSubSection
{
    public override LocalisableString Title => strings.Offset;
    public override IconUsage Icon => FontAwesome6.Solid.Clock;

    private SettingsAudioStrings strings => LocalizationStrings.Settings.Audio;

    [BackgroundDependencyLoader]
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

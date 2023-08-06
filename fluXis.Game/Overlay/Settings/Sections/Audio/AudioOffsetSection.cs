using fluXis.Game.Configuration;
using fluXis.Game.Overlay.Settings.UI;
using fluXis.Game.Screens;
using fluXis.Game.Screens.Offset;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Game.Overlay.Settings.Sections.Audio;

public partial class AudioOffsetSection : SettingsSubSection
{
    public override string Title => "Offset";

    [BackgroundDependencyLoader]
    private void load(FluXisGameBase game)
    {
        AddRange(new Drawable[]
        {
            new SettingsSlider<float>
            {
                Label = "Audio Offset",
                ValueLabel = "{value}ms",
                Description = "Changes the audio offset for all songs.",
                Bindable = Config.GetBindable<float>(FluXisSetting.GlobalOffset),
                Step = 1
            },
            new SettingsButton
            {
                Label = "Open Offset wizard",
                ButtonText = "Open",
                Action = () =>
                {
                    if (game.ScreenStack.CurrentScreen is OffsetSetup or not FluXisScreen { AllowExit: true }) return;

                    game.Settings.Hide();
                    game.ScreenStack.Push(new OffsetSetup());
                }
            }
        });
    }
}

using fluXis.Game.Configuration.Experiments;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace fluXis.Game.Overlay.Settings.Sections;

public partial class ExperimentsSection : SettingsSection
{
    public override IconUsage Icon => FontAwesome6.Solid.Flask;
    public override LocalisableString Title => "Experiments";

    [BackgroundDependencyLoader(true)]
    private void load(ExperimentConfigManager experiments)
    {
        AddRange(new Drawable[]
        {
            new SettingsToggle
            {
                Label = "Enable Design Tab",
                Bindable = experiments.GetBindable<bool>(ExperimentConfig.DesignTab)
            },
            new SettingsToggle
            {
                Label = "Enable Storyboarding Tab",
                Bindable = experiments.GetBindable<bool>(ExperimentConfig.StoryboardTab)
            },
            new SettingsToggle
            {
                Label = "Seeking in replays",
                Description = "Might make gameplay with a lot of effects more laggy.",
                Bindable = experiments.GetBindable<bool>(ExperimentConfig.Seeking)
            }
        });
    }
}

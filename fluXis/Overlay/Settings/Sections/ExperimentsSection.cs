using fluXis.Configuration.Experiments;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;

namespace fluXis.Overlay.Settings.Sections;

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
                Label = "Enable Storyboarding Tab",
                Bindable = experiments.GetBindable<bool>(ExperimentConfig.StoryboardTab)
            }
        });
    }
}

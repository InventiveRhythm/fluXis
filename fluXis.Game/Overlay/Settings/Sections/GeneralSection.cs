using fluXis.Game.Configuration;
using fluXis.Game.Graphics;
using fluXis.Game.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Overlay.Settings.Sections;

public partial class GeneralSection : SettingsSection
{
    public override IconUsage Icon => FontAwesome.Solid.Cog;
    public override string Title => "General";

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config)
    {
        AddRange(new Drawable[]
        {
            new Container
            {
                Height = 200,
                RelativeSizeAxes = Axes.X,
                Children = new Drawable[]
                {
                    new FluXisSpriteText
                    {
                        Text = "fluXis",
                        FontSize = 50,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.BottomCentre,
                    },
                    new FluXisSpriteText
                    {
                        Text = FluXisGameBase.VersionString,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.TopCentre
                    },
                }
            },
            new SettingsToggle
            {
                Label = "Import maps from other games",
                Description = "Requires a restart to take effect.",
                Bindable = config.GetBindable<bool>(FluXisSetting.ImportOtherGames)
            }
        });
    }
}

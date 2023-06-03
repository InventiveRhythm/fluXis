using fluXis.Game.Graphics;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Overlay.Settings.Sections;

public partial class GeneralSection : SettingsSection
{
    public override IconUsage Icon => FontAwesome.Solid.Cog;
    public override string Title => "General";

    public GeneralSection()
    {
        Add(new Container
        {
            Height = 510,
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
        });
    }
}

using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Overlay.Settings.Sections.General;
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
    private void load()
    {
        AddRange(new Drawable[]
        {
            new GeneralLanguageSection(),
            Divider,
            new GeneralUpdatesSection(),
            Divider,
            new GeneralFoldersSection(),
            new Container
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Margin = new MarginPadding { Top = 20 },
                Children = new Drawable[]
                {
                    new FluXisSpriteText
                    {
                        Text = "fluXis",
                        FontSize = 32,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.BottomCentre
                    },
                    new FluXisSpriteText
                    {
                        Text = FluXisGameBase.VersionString,
                        Colour = FluXisColors.Text2,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.TopCentre
                    }
                }
            }
        });
    }
}

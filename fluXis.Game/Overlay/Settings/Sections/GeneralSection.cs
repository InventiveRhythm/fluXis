using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Overlay.Settings.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Platform;

namespace fluXis.Game.Overlay.Settings.Sections;

public partial class GeneralSection : SettingsSection
{
    public override IconUsage Icon => FontAwesome.Solid.Cog;
    public override string Title => "General";

    [BackgroundDependencyLoader]
    private void load(Storage storage)
    {
        AddRange(new Drawable[]
        {
            new SettingsButton
            {
                Label = "Check for updates",
                Enabled = false,
                ButtonText = "Check"
            },
            new SettingsButton
            {
                Label = "Open fluXis folder",
                ButtonText = "Open",
                Action = () => storage.OpenFileExternally(".")
            },
            new SettingsButton
            {
                Label = "Change folder location",
                Enabled = false,
                ButtonText = "Change"
            },
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

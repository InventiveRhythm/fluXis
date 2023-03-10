using System.Reflection;
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
        var version = Assembly.GetEntryAssembly()?.GetName().Version;

        Add(new Container
        {
            Height = 510,
            RelativeSizeAxes = Axes.X,
            Children = new Drawable[]
            {
                new SpriteText
                {
                    Text = "fluXis",
                    Font = new FontUsage("Quicksand", 50, "Bold"),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.BottomCentre,
                },
                new SpriteText
                {
                    Text = "v" + (version?.ToString(3) ?? "unknown"),
                    Font = new FontUsage("Quicksand", 20, "Bold"),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.TopCentre
                },
            }
        });
    }
}

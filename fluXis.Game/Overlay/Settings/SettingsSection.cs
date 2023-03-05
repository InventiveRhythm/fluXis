using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Game.Overlay.Settings;

public partial class SettingsSection : FillFlowContainer
{
    public virtual IconUsage Icon { get; set; } = FontAwesome.Solid.Cog;
    public virtual string Title { get; set; } = "Section";

    public SettingsSection()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Direction = FillDirection.Vertical;
        Spacing = new Vector2(0, 10);
    }
}

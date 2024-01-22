using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Screens.Edit.Tabs.Charting.Placement;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Tools;

public class SelectTool : ChartingTool
{
    public override string Name => "Select";
    public override string Description => "Select and move objects";
    public override Drawable CreateIcon() => new SpriteIcon { Icon = FontAwesome6.Solid.ArrowPointer };
    public override PlacementBlueprint CreateBlueprint() => null;
}

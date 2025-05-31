using fluXis.Graphics.Sprites.Icons;
using fluXis.Screens.Edit.Tabs.Charting.Blueprints.Placement;
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.Tabs.Charting.Tools;

public class SelectTool : ChartingTool
{
    public override string Name => "Select";
    public override string Description => "Select and move objects";
    public override Drawable CreateIcon() => new FluXisSpriteIcon { Icon = FontAwesome6.Solid.ArrowPointer };
    public override PlacementBlueprint CreateBlueprint() => null;
}

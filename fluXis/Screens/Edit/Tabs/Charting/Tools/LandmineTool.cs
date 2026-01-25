using fluXis.Graphics.Sprites.Icons;
using fluXis.Screens.Edit.Tabs.Charting.Blueprints.Placement;
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.Tabs.Charting.Tools;

public class LandmineTool : ChartingTool
{
    public override string Name => "Landmine";
    public override string Description => "Creates a landmine.";
    public override Drawable CreateIcon() => new FluXisSpriteIcon { Icon = FontAwesome6.Solid.Bomb };
    public override PlacementBlueprint CreateBlueprint() => new LandminePlacementBlueprint();
}

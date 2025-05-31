using fluXis.Graphics.Sprites.Icons;
using fluXis.Screens.Edit.Tabs.Charting.Blueprints.Placement;
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.Tabs.Charting.Tools;

public class TickNoteTool : ChartingTool
{
    public override string Name => "Tick Note";
    public override string Description => "Creates a tick note.";
    public override Drawable CreateIcon() => new FluXisSpriteIcon { Icon = FontAwesome6.Solid.AngleDown };
    public override PlacementBlueprint CreateBlueprint() => new TickNotePlacementBlueprint();
}

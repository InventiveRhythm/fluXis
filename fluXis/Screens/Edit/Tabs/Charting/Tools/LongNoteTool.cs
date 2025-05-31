using fluXis.Graphics.Sprites.Icons;
using fluXis.Screens.Edit.Tabs.Charting.Blueprints.Placement;
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.Tabs.Charting.Tools;

public class LongNoteTool : ChartingTool
{
    public override string Name => "Long Note";
    public override string Description => "Creates a long note.";
    public override Drawable CreateIcon() => new FluXisSpriteIcon { Icon = FontAwesome6.Solid.PenRuler };
    public override PlacementBlueprint CreateBlueprint() => new LongNotePlacementBlueprint();
}

using fluXis.Graphics.Sprites;
using fluXis.Screens.Edit.Tabs.Charting.Blueprints.Placement;
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.Tabs.Charting.Tools;

public class SingleNoteTool : ChartingTool
{
    public override string Name => "Single Note";
    public override string Description => "Creates a single note.";
    public override Drawable CreateIcon() => new FluXisSpriteIcon { Icon = FontAwesome6.Solid.Pen };
    public override PlacementBlueprint CreateBlueprint() => new SingleNotePlacementBlueprint();
}

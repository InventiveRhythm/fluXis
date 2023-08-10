using fluXis.Game.Screens.Edit.Tabs.Charting.Placement;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Tools;

public class SingleNoteTool : ChartingTool
{
    public override string Name => "Single Note";
    public override PlacementBlueprint CreateBlueprint() => new SingleNotePlacementBlueprint();
}

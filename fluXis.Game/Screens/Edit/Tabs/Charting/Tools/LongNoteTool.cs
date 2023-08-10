using fluXis.Game.Screens.Edit.Tabs.Charting.Placement;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Tools;

public class LongNoteTool : ChartingTool
{
    public override string Name => "Long Note";
    public override PlacementBlueprint CreateBlueprint() => new LongNotePlacementBlueprint();
}

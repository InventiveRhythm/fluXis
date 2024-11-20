using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Screens.Edit.Tabs.Charting.Blueprints.Placement;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Tools;

public class SingleNoteTool : ChartingTool
{
    public override string Name => "Single Note";
    public override string Description => "Creates a single note.";
    public override Drawable CreateIcon() => new FluXisSpriteIcon { Icon = FontAwesome6.Solid.Pen };
    public override PlacementBlueprint CreateBlueprint() => new SingleNotePlacementBlueprint();
}

using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Screens.Edit.Tabs.Charting.Blueprints.Placement;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Tools;

public class TickNoteTool : ChartingTool
{
    public override string Name => "Tick Note";
    public override string Description => "Creates a tick note.";
    public override Drawable CreateIcon() => new SpriteIcon { Icon = FontAwesome6.Solid.AngleDown };
    public override PlacementBlueprint CreateBlueprint() => new TickNotePlacementBlueprint();
}

using fluXis.Game.Screens.Edit.Tabs.Charting.Placement;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Tools;

public class TickNoteTool : ChartingTool
{
    public override string Name => "Tick Note";
    public override string Description => "Creates a tick note.";
    public override Drawable CreateIcon() => new SpriteIcon { Icon = FontAwesome.Solid.ChevronDown };
    public override PlacementBlueprint CreateBlueprint() => new TickNotePlacementBlueprint();
}

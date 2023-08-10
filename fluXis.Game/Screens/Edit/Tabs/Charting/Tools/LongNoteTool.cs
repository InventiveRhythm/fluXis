using fluXis.Game.Screens.Edit.Tabs.Charting.Placement;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Tools;

public class LongNoteTool : ChartingTool
{
    public override string Name => "Long Note";
    public override string Description => "Creates a long note.";
    public override Drawable CreateIcon() => new SpriteIcon { Icon = FontAwesome.Solid.PencilRuler };
    public override PlacementBlueprint CreateBlueprint() => new LongNotePlacementBlueprint();
}

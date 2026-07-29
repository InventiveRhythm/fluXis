using fluXis.Graphics.Sprites.Icons;
using fluXis.Screens.Edit.Tabs.Charting.Blueprints.Placement;
using osu.Framework.Graphics;
using osu.Framework.Localisation;

namespace fluXis.Screens.Edit.Tabs.Charting.Tools;

public class TickNoteTool : ChartingTool
{
    public override LocalisableString Name => "Tick Note";
    public override LocalisableString Description => "Creates a tick note.";
    public override Drawable CreateIcon() => new FluXisSpriteIcon { Icon = Phosphor.Bold.CaretDown };
    public override PlacementBlueprint CreateBlueprint() => new TickNotePlacementBlueprint();
}

using fluXis.Graphics.Sprites.Icons;
using fluXis.Screens.Edit.Tabs.Charting.Blueprints.Placement;
using osu.Framework.Graphics;
using osu.Framework.Localisation;

namespace fluXis.Screens.Edit.Tabs.Charting.Tools;

public class SingleNoteTool : ChartingTool
{
    public override LocalisableString Name => "Single Note";
    public override LocalisableString Description => "Creates a single note.";
    public override Drawable CreateIcon() => new FluXisSpriteIcon { Icon = Phosphor.Bold.PencilSimple };
    public override PlacementBlueprint CreateBlueprint() => new SingleNotePlacementBlueprint();
}

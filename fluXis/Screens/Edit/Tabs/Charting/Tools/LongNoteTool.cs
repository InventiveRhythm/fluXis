using fluXis.Graphics.Sprites.Icons;
using fluXis.Screens.Edit.Tabs.Charting.Blueprints.Placement;
using osu.Framework.Graphics;
using osu.Framework.Localisation;

namespace fluXis.Screens.Edit.Tabs.Charting.Tools;

public class LongNoteTool : ChartingTool
{
    public override LocalisableString Name => "Long Note";
    public override LocalisableString Description => "Creates a long note.";
    public override Drawable CreateIcon() => new FluXisSpriteIcon { Icon = Phosphor.Bold.Ruler };
    public override PlacementBlueprint CreateBlueprint() => new LongNotePlacementBlueprint();
}

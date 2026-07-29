using fluXis.Graphics.Sprites.Icons;
using fluXis.Screens.Edit.Tabs.Charting.Blueprints.Placement;
using osu.Framework.Graphics;
using osu.Framework.Localisation;

namespace fluXis.Screens.Edit.Tabs.Charting.Tools;

public class LandmineTool : ChartingTool
{
    public override LocalisableString Name => "Landmine";
    public override LocalisableString Description => "Creates a landmine.";
    public override Drawable CreateIcon() => new FluXisSpriteIcon { Icon = Phosphor.Bold.Bomb };
    public override PlacementBlueprint CreateBlueprint() => new LandminePlacementBlueprint();
}

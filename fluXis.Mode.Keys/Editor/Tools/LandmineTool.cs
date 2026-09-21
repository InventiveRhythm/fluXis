using fluXis.Graphics.Sprites.Icons;
using fluXis.Mode.Keys.Editor.Blueprint;
using fluXis.Screens.Edit.Tabs.Charting.Blueprints.Placement;
using fluXis.Screens.Edit.Tabs.Charting.Tools;
using osu.Framework.Graphics;
using osu.Framework.Localisation;

namespace fluXis.Mode.Keys.Editor.Tools;

public class LandmineTool : ChartingTool
{
    public override LocalisableString Name => "Landmine";
    public override LocalisableString Description => "Creates a landmine.";
    public override Drawable CreateIcon() => new FluXisSpriteIcon { Icon = Phosphor.Bold.Bomb };
    public override PlacementBlueprint CreateBlueprint() => new LandminePlacementBlueprint();
}

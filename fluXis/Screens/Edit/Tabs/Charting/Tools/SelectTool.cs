using fluXis.Graphics.Sprites.Icons;
using fluXis.Screens.Edit.Tabs.Charting.Blueprints.Placement;
using osu.Framework.Graphics;
using osu.Framework.Localisation;

namespace fluXis.Screens.Edit.Tabs.Charting.Tools;

public class SelectTool : ChartingTool
{
    public override LocalisableString Name => "Select";
    public override LocalisableString Description => "Select and move objects";
    public override Drawable CreateIcon() => new FluXisSpriteIcon { Icon = Phosphor.Bold.Selection };
    public override PlacementBlueprint CreateBlueprint() => null;
}

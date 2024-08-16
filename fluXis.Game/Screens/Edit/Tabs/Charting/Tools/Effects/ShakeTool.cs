using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Screens.Edit.Tabs.Charting.Blueprints.Placement;
using fluXis.Game.Screens.Edit.Tabs.Charting.Blueprints.Placement.Effect;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Tools.Effects;

public class ShakeTool : EffectTool
{
    public override string Name => "Shake";
    public override string Description => "Shakes the screen.";
    public override string Letter => "S";
    public override Drawable CreateIcon() => new FluXisIcon { Type = FluXisIconType.Shake };
    public override PlacementBlueprint CreateBlueprint() => new ShakePlacementBlueprint();
}

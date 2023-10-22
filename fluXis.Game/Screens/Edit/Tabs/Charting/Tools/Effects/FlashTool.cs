using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Screens.Edit.Tabs.Charting.Placement;
using fluXis.Game.Screens.Edit.Tabs.Charting.Placement.Effect;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Tools.Effects;

public class FlashTool : EffectTool
{
    public override string Name => "Flash";
    public override string Description => "Creates a flash that fills the entire screen.";
    public override string Letter => "F";
    public override Drawable CreateIcon() => new FluXisIcon { Type = FluXisIconType.Flash };
    public override PlacementBlueprint CreateBlueprint() => new FlashPlacementBlueprint();
}

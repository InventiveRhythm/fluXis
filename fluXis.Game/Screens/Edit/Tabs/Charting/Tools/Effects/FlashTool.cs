using fluXis.Game.Screens.Edit.Tabs.Charting.Placement;
using fluXis.Game.Screens.Edit.Tabs.Charting.Placement.Effect;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Tools.Effects;

public class FlashTool : EffectTool
{
    public override string Name => "Flash";
    public override string Description => "Creates a screen flash that fills the entire screen.";
    public override string Letter => "F";
    public override Drawable CreateIcon() => new SpriteIcon { Icon = FontAwesome.Solid.Lightbulb };
    public override PlacementBlueprint CreateBlueprint() => new FlashPlacementBlueprint();
}

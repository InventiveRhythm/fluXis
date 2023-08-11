using fluXis.Game.Screens.Edit.Tabs.Charting.Placement;
using fluXis.Game.Screens.Edit.Tabs.Charting.Placement.Effect;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Tools.Effects;

public class LaneSwitchTool : EffectTool
{
    public override string Name => "Lane Switch";
    public override string Description => "Changes the amount of lanes mid-song.";
    public override string Letter => "L";
    public override Drawable CreateIcon() => new SpriteIcon { Icon = FontAwesome.Solid.ArrowsAltH };
    public override PlacementBlueprint CreateBlueprint() => new LaneSwitchPlacementBlueprint();
}

using fluXis.Graphics.Sprites.Icons;
using fluXis.Mode.Keys.Editor.Blueprint;
using fluXis.Screens.Edit.Tabs.Charting.Blueprints.Placement;
using fluXis.Screens.Edit.Tabs.Charting.Tools;
using osu.Framework.Graphics;
using osu.Framework.Localisation;

namespace fluXis.Mode.Keys.Editor.Tools;

public class TickNoteTool : ChartingTool
{
    public override LocalisableString Name => "Tick Note";
    public override LocalisableString Description => "Creates a tick note.";
    public override Drawable CreateIcon() => new FluXisSpriteIcon { Icon = Phosphor.Bold.CaretDown };
    public override PlacementBlueprint CreateBlueprint() => new TickNotePlacementBlueprint();
}

using fluXis.Graphics.Sprites.Icons;
using fluXis.Modes.Keys.Editor.Blueprint;
using fluXis.Screens.Edit.Tabs.Charting.Blueprints.Placement;
using fluXis.Screens.Edit.Tabs.Charting.Tools;
using osu.Framework.Graphics;
using osu.Framework.Localisation;

namespace fluXis.Modes.Keys.Editor.Tools;

public class SingleNoteTool : ChartingTool
{
    public override LocalisableString Name => "Single Note";
    public override LocalisableString Description => "Creates a single note.";
    public override Drawable CreateIcon() => new FluXisSpriteIcon { Icon = Phosphor.Bold.PencilSimple };
    public override PlacementBlueprint CreateBlueprint() => new SingleNotePlacementBlueprint();
}

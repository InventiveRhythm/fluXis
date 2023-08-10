using fluXis.Game.Screens.Edit.Tabs.Charting.Placement;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Tools;

public abstract class ChartingTool
{
    public abstract string Name { get; }
    public abstract string Description { get; }
    public abstract PlacementBlueprint CreateBlueprint();
    public virtual Drawable CreateIcon() => null;
    public override string ToString() => Name;
}

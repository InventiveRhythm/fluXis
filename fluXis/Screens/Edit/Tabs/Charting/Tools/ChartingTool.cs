using fluXis.Graphics.UserInterface.Color;
using fluXis.Screens.Edit.Tabs.Charting.Blueprints.Placement;
using osu.Framework.Graphics;
using osuTK.Input;

namespace fluXis.Screens.Edit.Tabs.Charting.Tools;

public abstract class ChartingTool
{
    public abstract string Name { get; }
    public abstract string Description { get; }
    public abstract PlacementBlueprint CreateBlueprint();
    public virtual Drawable CreateIcon() => null;
    public virtual Key Shortcut => Key.Unknown;
    public virtual Colour4 Color => Theme.Text;
    public override string ToString() => Name;
}

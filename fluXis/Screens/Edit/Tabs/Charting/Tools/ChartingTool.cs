using fluXis.Graphics.UserInterface.Color;
using fluXis.Screens.Edit.Tabs.Charting.Blueprints.Placement;
using osu.Framework.Graphics;
using osu.Framework.Localisation;
using osuTK.Input;

namespace fluXis.Screens.Edit.Tabs.Charting.Tools;

public abstract class ChartingTool
{
    public abstract LocalisableString Name { get; }
    public abstract LocalisableString Description { get; }
    public abstract PlacementBlueprint CreateBlueprint();
    public virtual Drawable CreateIcon() => null;
    public virtual Key Shortcut => Key.Unknown;
    public virtual Colour4 Color => Theme.Text;
}

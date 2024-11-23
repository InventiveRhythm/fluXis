using fluXis.Game.Map.Structures.Bases;
using fluXis.Game.Screens.Edit.Blueprints.Selection;
using fluXis.Game.Screens.Edit.Tabs.Charting.Playfield;
using osu.Framework.Allocation;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Blueprints.Selection;

public partial class ChartingSelectionBlueprint : SelectionBlueprint<ITimedObject>
{
    [Resolved]
    protected EditorSnapProvider Snaps { get; private set; }

    [Resolved]
    protected EditorHitObjectContainer HitObjectContainer { get; private set; }

    public override double FirstComparer => Object.Time;
    public override double SecondComparer => Object.Time;

    protected ChartingSelectionBlueprint(ITimedObject info)
        : base(info)
    {
    }
}

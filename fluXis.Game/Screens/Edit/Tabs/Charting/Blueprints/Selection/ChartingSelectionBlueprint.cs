using fluXis.Game.Map.Structures.Bases;
using fluXis.Game.Screens.Edit.Blueprints.Selection;
using fluXis.Game.Screens.Edit.Tabs.Charting.Playfield;
using osu.Framework.Allocation;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Blueprints.Selection;

public partial class ChartingSelectionBlueprint : SelectionBlueprint<ITimedObject>
{
    [Resolved]
    private EditorPlayfield playfield { get; set; }

    protected EditorHitObjectContainer HitObjectContainer => playfield.HitObjectContainer;

    public override double FirstComparer => Object.Time;
    public override double SecondComparer => Object.Time;

    protected ChartingSelectionBlueprint(ITimedObject info)
        : base(info)
    {
    }
}

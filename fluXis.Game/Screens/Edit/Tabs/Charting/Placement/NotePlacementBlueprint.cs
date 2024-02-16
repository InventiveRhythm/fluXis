using System;
using fluXis.Game.Map.Structures;
using fluXis.Game.Screens.Edit.Actions.Notes;
using osu.Framework.Allocation;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Placement;

public partial class NotePlacementBlueprint : PlacementBlueprint
{
    [Resolved]
    private ChartingContainer chartingContainer { get; set; }

    protected HitObject Hit => Object as HitObject;

    protected NotePlacementBlueprint()
        : base(new HitObject())
    {
    }

    public override void UpdatePlacement(float time, int lane)
    {
        base.UpdatePlacement(time, lane);
        ((HitObject)Object).Lane = Math.Clamp(lane, 1, EditorValues.Editor.Map.KeyCount);
    }

    public override void OnPlacementFinished(bool commit)
    {
        if (!commit)
            return;

        Hit.HitSound = chartingContainer.CurrentHitSound.Value;
        EditorValues.ActionStack.Add(new NotePlaceAction(Hit, EditorValues.MapInfo));
    }
}

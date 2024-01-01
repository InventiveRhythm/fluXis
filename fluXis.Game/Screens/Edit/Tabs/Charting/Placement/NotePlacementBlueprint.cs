using System;
using fluXis.Game.Map.Structures;
using fluXis.Game.Screens.Edit.Actions.Notes;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Placement;

public partial class NotePlacementBlueprint : PlacementBlueprint
{
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
        if (commit)
            EditorValues.ActionStack.Add(new NotePlaceAction((HitObject)Object, EditorValues.MapInfo));
    }
}

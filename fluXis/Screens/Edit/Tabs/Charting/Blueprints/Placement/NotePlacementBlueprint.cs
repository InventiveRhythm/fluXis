using System;
using fluXis.Map.Structures;
using fluXis.Screens.Edit.Actions.Generic;
using Midori.Utils;
using osu.Framework.Allocation;
using osu.Framework.Input;

namespace fluXis.Screens.Edit.Tabs.Charting.Blueprints.Placement;

public partial class NotePlacementBlueprint<T> : PlacementBlueprint
    where T : HitObject, new()
{
    [Resolved]
    private ChartingContainer chartingContainer { get; set; }

    protected T Hit => Object as T;

    private InputManager input;

    protected NotePlacementBlueprint()
        : base(new T())
    {
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        input = GetContainingInputManager();
    }

    public override void UpdatePlacement(double time, int lane)
    {
        base.UpdatePlacement(time, lane);
        ((HitObject)Object).Lane = Math.Clamp(lane, 1, Configuration.ColumnCount);
    }

    protected override void OnPlacementFinished(bool commit)
    {
        if (!commit)
            return;

        Hit.Sample = chartingContainer.CurrentHitSound.Value;

        if (input.CurrentState.Keyboard.ShiftPressed && Map.Playable.IsDualSplit)
        {
            var clone = Hit.JsonCopy();

            if (clone.Lane > Map.RealmMap.KeyCount)
                clone.Lane -= Map.RealmMap.KeyCount;
            else
                clone.Lane += Map.RealmMap.KeyCount;

            Actions.Add(new ObjectMultiPlaceAction<HitObject>([Hit, clone]));
            return;
        }

        Actions.Add(new ObjectPlaceAction<HitObject>(Hit));
    }
}

using System;
using fluXis.Map.Structures;
using fluXis.Screens.Edit.Actions.Notes;
using fluXis.Screens.Edit.Tabs.Verify;
using fluXis.Utils;
using osu.Framework.Allocation;
using osu.Framework.Input;

namespace fluXis.Screens.Edit.Tabs.Charting.Blueprints.Placement;

public partial class NotePlacementBlueprint : PlacementBlueprint
{
    [Resolved]
    private ChartingContainer chartingContainer { get; set; }

    protected override ITimePositionProvider PositionProvider
    {
        get
        {
            var index = (Hit.Lane - 1) / Map.RealmMap.KeyCount;
            return ChartingContainer.Playfields[index];
        }
    }

    protected HitObject Hit => Object as HitObject;

    private InputManager input;

    protected NotePlacementBlueprint()
        : base(new HitObject())
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
        ((HitObject)Object).Lane = Math.Clamp(lane, 1, ((IVerifyContext)Map).MaxKeyCount);
    }

    protected override void OnPlacementFinished(bool commit)
    {
        if (!commit)
            return;

        Hit.HitSound = chartingContainer.CurrentHitSound.Value;

        if (input.CurrentState.Keyboard.ShiftPressed && Map.MapInfo.IsSplit)
        {
            var clone = Hit.JsonCopy();

            if (clone.Lane > Map.RealmMap.KeyCount)
                clone.Lane -= Map.RealmMap.KeyCount;
            else
                clone.Lane += Map.RealmMap.KeyCount;

            Actions.Add(new NoteMultiPlaceAction(new[] { Hit, clone }));
            return;
        }

        Actions.Add(new NotePlaceAction(Hit));
    }
}

﻿using System.Linq;
using fluXis.Screens.Edit.Blueprints;
using fluXis.Screens.Edit.Blueprints.Selection;
using fluXis.Storyboards;
using osu.Framework.Allocation;
using osu.Framework.Input.Events;
using osuTK.Input;

namespace fluXis.Screens.Edit.Tabs.Storyboarding.Timeline.Blueprints;

public partial class TimelineBlueprintContainer : BlueprintContainer<StoryboardElement>
{
    protected override bool HorizontalSelection => true;

    [Resolved]
    private Storyboard storyboard { get; set; }

    [Resolved]
    private StoryboardTimeline timeline { get; set; }

    [Resolved]
    private EditorSnapProvider snaps { get; set; }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        storyboard.ElementAdded += AddBlueprint;
        storyboard.ElementRemoved += RemoveBlueprint;
        storyboard.Elements.ForEach(AddBlueprint);
    }

    protected override SelectionBlueprint<StoryboardElement> CreateBlueprint(StoryboardElement element)
    {
        var bp = new TimelineElementBlueprint(element);
        bp.Drawable = timeline.GetDrawable(element);
        return bp;
    }

    protected override bool OnMouseDown(MouseDownEvent e) => e.Button == MouseButton.Left && base.OnMouseDown(e);

    protected override void MoveSelection(DragEvent e)
    {
        if (DraggedBlueprints == null) return;

        var delta = e.ScreenSpaceMousePosition - e.ScreenSpaceMouseDownPosition;

        var position = DraggedBlueprintsPositions.First() + delta;
        var time = timeline.TimeAtScreenSpacePosition(position);
        int lane = timeline.ZAtScreenSpacePosition(position);
        var snappedTime = snaps.SnapTime(time, true);

        var timeDelta = snappedTime - DraggedBlueprints.First().Object.StartTime;

        int zDelta = lane - DraggedBlueprints.First().Object.ZIndex;
        var minLane = DraggedBlueprints.Min(b => b.Object.ZIndex);

        if (minLane + zDelta < 0)
            zDelta = 0;

        foreach (var blueprint in DraggedBlueprints)
        {
            blueprint.Object.StartTime += timeDelta;
            blueprint.Object.EndTime += timeDelta;
            blueprint.Object.ZIndex += zDelta;
        }
    }
}

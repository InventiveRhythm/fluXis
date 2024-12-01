using System;
using System.Linq;
using fluXis.Game.Map.Structures;
using fluXis.Game.Map.Structures.Bases;
using fluXis.Game.Map.Structures.Events;
using fluXis.Game.Screens.Edit.Blueprints;
using fluXis.Game.Screens.Edit.Blueprints.Selection;
using fluXis.Game.Screens.Edit.Tabs.Charting.Blueprints.Placement;
using fluXis.Game.Screens.Edit.Tabs.Charting.Blueprints.Selection;
using fluXis.Game.Screens.Edit.Tabs.Charting.Blueprints.Selection.Effect;
using fluXis.Game.Screens.Edit.Tabs.Charting.Tools;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Blueprints;

public partial class ChartingBlueprintContainer : BlueprintContainer<ITimedObject>
{
    protected override bool InArea => ChartingContainer.CursorInPlacementArea;

    [Resolved]
    private EditorMap map { get; set; }

    [Resolved]
    private EditorSnapProvider snaps { get; set; }

    public ChartingContainer ChartingContainer { get; init; }

    public ChartingTool CurrentTool
    {
        get => currentTool;
        set
        {
            currentTool = value;
            removePlacement();

            CurrentToolChanged?.Invoke();
        }
    }

    public event Action CurrentToolChanged;
    private ChartingTool currentTool;

    private PlacementBlueprint currentPlacement;
    private Container placementContainer;

    [BackgroundDependencyLoader]
    private void load()
    {
        currentTool = ChartingContainer.Tools[0] as SelectTool;

        AddInternal(placementContainer = new Container { RelativeSizeAxes = Axes.Both });
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        map.HitObjectAdded += AddBlueprint;
        map.HitObjectRemoved += RemoveBlueprint;

        map.FlashEventAdded += AddBlueprint;
        map.FlashEventRemoved += RemoveBlueprint;

        SelectionBlueprints.StartBulk();

        foreach (var hitObject in ChartingContainer.HitObjects)
            AddBlueprint(hitObject.Data);

        foreach (var flash in ChartingContainer.Playfield.Effects.Flashes)
            AddBlueprint(flash.FlashEvent);

        SelectionBlueprints.EndBulk();
    }

    protected override void Update()
    {
        base.Update();

        if (currentPlacement != null)
        {
            switch (currentPlacement.State)
            {
                case PlacementState.Waiting:
                    if (!ChartingContainer.CursorInPlacementArea)
                        removePlacement();
                    break;

                case PlacementState.Completed:
                    removePlacement();
                    break;
            }
        }

        if (ChartingContainer.CursorInPlacementArea)
            createPlacement();

        if (currentPlacement != null)
            updatePlacementPosition();
    }

    protected override SelectionHandler<ITimedObject> CreateSelectionHandler() => new ChartingSelectionHandler();

    private void createPlacement()
    {
        if (currentPlacement != null) return;

        var blueprint = CurrentTool?.CreateBlueprint();

        if (blueprint != null)
            placementContainer.Child = currentPlacement = blueprint;
    }

    private void updatePlacementPosition()
    {
        var hitObjectContainer = ChartingContainer.Playfield.HitObjectContainer;
        var mousePosition = InputManager.CurrentState.Mouse.Position;

        var time = snaps.SnapTime(hitObjectContainer.TimeAtScreenSpacePosition(mousePosition));
        var lane = hitObjectContainer.LaneAtScreenSpacePosition(mousePosition);
        currentPlacement.UpdatePlacement(time, lane);
    }

    private void removePlacement()
    {
        currentPlacement?.FinishPlacement(false);
        currentPlacement?.Expire();
        currentPlacement = null;
    }

    protected override SelectionBlueprint<ITimedObject> CreateBlueprint(ITimedObject obj)
    {
        SelectionBlueprint<ITimedObject> blueprint = null!;

        switch (obj)
        {
            case HitObject hit:
                var hitDrawable = hit.EditorDrawable;
                if (hitDrawable == null) return null;

                blueprint = hit.LongNote ? new LongNoteSelectionBlueprint(hit) : new SingleNoteSelectionBlueprint(hit);
                blueprint.Drawable = hitDrawable;
                break;

            case FlashEvent flash:
                var flashDrawable = ChartingContainer.Playfield.Effects.Flashes.FirstOrDefault(d => d.FlashEvent == obj);
                if (flashDrawable == null) return null;

                blueprint = new FlashSelectionBlueprint(flash);
                blueprint.Drawable = flashDrawable;
                break;
        }

        return blueprint;
    }

    protected override void MoveSelection(DragEvent e)
    {
        if (DraggedBlueprints == null) return;

        var delta = e.ScreenSpaceMousePosition - e.ScreenSpaceMouseDownPosition;

        var position = DraggedBlueprintsPositions.First() + delta;
        var time = ChartingContainer.Playfield.HitObjectContainer.TimeAtScreenSpacePosition(position);
        int lane = ChartingContainer.Playfield.HitObjectContainer.LaneAtScreenSpacePosition(position);
        var snappedTime = snaps.SnapTime(time, true);

        var timeDelta = snappedTime - DraggedBlueprints.First().Object.Time;
        int laneDelta = 0;

        if (DraggedBlueprints.Any(x => x is NoteSelectionBlueprint))
        {
            var hitBlueprints = DraggedBlueprints.OfType<NoteSelectionBlueprint>().ToArray();

            laneDelta = lane - hitBlueprints.First().Object.Lane;

            var minLane = hitBlueprints.Min(b => b.Object.Lane);
            var maxLane = hitBlueprints.Max(b => b.Object.Lane);

            if (minLane + laneDelta <= 0 || maxLane + laneDelta > map.RealmMap.KeyCount)
                laneDelta = 0;
        }

        foreach (var blueprint in DraggedBlueprints)
        {
            blueprint.Object.Time += timeDelta;

            if (blueprint is NoteSelectionBlueprint noteBlueprint)
                noteBlueprint.Object.Lane += laneDelta;
        }
    }
}

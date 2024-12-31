using System;
using System.Linq;
using fluXis.Map.Structures;
using fluXis.Map.Structures.Bases;
using fluXis.Map.Structures.Events;
using fluXis.Screens.Edit.Actions;
using fluXis.Screens.Edit.Actions.Notes;
using fluXis.Screens.Edit.Blueprints;
using fluXis.Screens.Edit.Blueprints.Selection;
using fluXis.Screens.Edit.Tabs.Charting.Blueprints.Placement;
using fluXis.Screens.Edit.Tabs.Charting.Blueprints.Selection;
using fluXis.Screens.Edit.Tabs.Charting.Blueprints.Selection.Effect;
using fluXis.Screens.Edit.Tabs.Charting.Tools;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Screens.Edit.Tabs.Charting.Blueprints;

public partial class ChartingBlueprintContainer : BlueprintContainer<ITimedObject>
{
    protected override bool InArea => ChartingContainer.CursorInPlacementArea;

    [Resolved]
    private EditorMap map { get; set; }

    [Resolved]
    private EditorActionStack actions { get; set; }

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

    [CanBeNull]
    private NoteMoveAction moveAction;

    protected override void StartedMoving()
    {
        moveAction = new NoteMoveAction(SelectedObjects.OfType<HitObject>().ToArray());
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

        var hitBlueprints = DraggedBlueprints.OfType<NoteSelectionBlueprint>().ToArray();

        if (hitBlueprints.Length != 0)
        {
            laneDelta = lane - hitBlueprints.First().Object.Lane;

            var minLane = hitBlueprints.Min(b => b.Object.Lane);
            var maxLane = hitBlueprints.Max(b => b.Object.Lane);

            if (minLane + laneDelta <= 0 || maxLane + laneDelta > map.RealmMap.KeyCount)
                laneDelta = 0;
        }

        var hits = hitBlueprints.Select(b => b.HitObject.Data).ToArray();
        var vecs = NoteMoveAction.CreateFrom(hits);
        moveAction?.Apply(vecs.Select(v => new Vector2d(v.X + timeDelta, v.Y + laneDelta)).ToArray(), true);
    }

    protected override void FinishedMoving()
    {
        base.FinishedMoving();

        if (moveAction is not null)
        {
            actions.Add(moveAction);
            moveAction = null;
        }
    }
}

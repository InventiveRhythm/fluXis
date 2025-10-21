using System;
using System.Linq;
using fluXis.Map.Structures;
using fluXis.Screens.Edit.Actions;
using fluXis.Screens.Edit.Actions.Notes;
using fluXis.Screens.Edit.Blueprints;
using fluXis.Screens.Edit.Blueprints.Selection;
using fluXis.Screens.Edit.Tabs.Charting.Blueprints.Placement;
using fluXis.Screens.Edit.Tabs.Charting.Blueprints.Selection;
using fluXis.Screens.Edit.Tabs.Charting.Tools;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace fluXis.Screens.Edit.Tabs.Charting.Blueprints;

public partial class ChartingBlueprintContainer : BlueprintContainer<HitObject>
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

        map.RegisterAddListener<HitObject>(AddBlueprint);
        map.RegisterRemoveListener<HitObject>(RemoveBlueprint);

        SelectionBlueprints.StartBulk();

        foreach (var hitObject in ChartingContainer.HitObjects)
            AddBlueprint(hitObject.Data);

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

    protected override SelectionHandler<HitObject> CreateSelectionHandler() => new ChartingSelectionHandler();

    private void createPlacement()
    {
        if (currentPlacement != null) return;

        var blueprint = CurrentTool?.CreateBlueprint();

        if (blueprint != null)
            placementContainer.Child = currentPlacement = blueprint;
    }

    private void updatePlacementPosition()
    {
        var playfield = ChartingContainer.Playfields.FirstOrDefault(p => p.CursorInPlacementArea);

        if (playfield is null)
            return;

        var container = playfield.HitObjectContainer;
        var mousePosition = InputManager.CurrentState.Mouse.Position;

        var time = snaps.SnapTime(container.TimeAtScreenSpacePosition(mousePosition));
        var lane = container.LaneAtScreenSpacePosition(mousePosition);
        currentPlacement.UpdatePlacement(time, lane);
    }

    private void removePlacement()
    {
        currentPlacement?.FinishPlacement(false);
        currentPlacement?.Expire();
        currentPlacement = null;
    }

    protected override bool OnMouseMove(MouseMoveEvent e)
    {
        if (currentPlacement is null
            || !currentPlacement.AllowPainting
            || !InputManager.CurrentState.Keyboard.ShiftPressed
            || !InputManager.CurrentState.Mouse.Buttons.Contains(MouseButton.Left))
            return base.OnMouseMove(e);

        currentPlacement.FinishPlacement(true);
        return true;
    }

    protected override SelectionBlueprint<HitObject> CreateBlueprint(HitObject hit)
    {
        var hitDrawable = hit.EditorDrawable;
        if (hitDrawable == null) return null;

        ChartingSelectionBlueprint blueprint = hit.LongNote
            ? new LongNoteSelectionBlueprint(hit)
            : new SingleNoteSelectionBlueprint(hit);

        blueprint.Drawable = hitDrawable;
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
        var time = ChartingContainer.Playfields[0].HitObjectContainer.TimeAtScreenSpacePosition(position);
        int lane = ChartingContainer.Playfields[0].HitObjectContainer.LaneAtScreenSpacePosition(position);
        var snappedTime = snaps.SnapTime(time, true);

        var timeDelta = snappedTime - DraggedBlueprints.First().Object.Time;
        int laneDelta = 0;

#pragma warning disable CA2021 // Rethrow to preserve stack details
        var hitBlueprints = DraggedBlueprints.OfType<NoteSelectionBlueprint>().ToArray();
#pragma warning restore CA2021

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

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        var timeDiff = e.Key switch
        {
            Key.Up => 1,
            Key.Down => -1,
            _ => 0
        };

        var laneDiff = e.Key switch
        {
            Key.Left => -1,
            Key.Right => 1,
            _ => 0
        };

        if (laneDiff != 0 || timeDiff != 0)
        {
            var selected = SelectedObjects.ToList();
            var changed = false;

            var action = new NoteMoveAction(selected.ToArray());

            var minLane = selected.Min(x => x.Lane);
            var maxLane = selected.Max(x => x.Lane);

            if (minLane + laneDiff >= 1 && maxLane + laneDiff <= map.RealmMap.KeyCount)
            {
                changed = true;
                selected.ForEach(x => x.Lane += laneDiff);
            }

            var minTime = selected.Min(x => x.Time);
            var step = snaps.CurrentStep * timeDiff;

            if (minTime + step >= 0)
            {
                changed = true;
                selected.ForEach(x => x.Time += step);
            }

            if (!changed)
                return false;

            action.Apply(NoteMoveAction.CreateFrom(selected.ToArray()), true);
            actions.Add(action);
            return true;
        }

        return base.OnKeyDown(e);
    }
}

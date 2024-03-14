using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Map.Events;
using fluXis.Game.Map.Structures;
using fluXis.Game.Overlay.Mouse;
using fluXis.Game.Screens.Edit.Tabs.Charting.Placement;
using fluXis.Game.Screens.Edit.Tabs.Charting.Selection;
using fluXis.Game.Screens.Edit.Tabs.Charting.Selection.Effect;
using fluXis.Game.Screens.Edit.Tabs.Charting.Tools;
using fluXis.Game.UI;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Blueprints;

public partial class BlueprintContainer : Container, ICursorDrag
{
    [Resolved]
    private EditorValues values { get; set; }

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

    protected readonly BindableList<ITimedObject> SelectedObjects = new();

    public SelectionBox SelectionBox { get; private set; }
    public SelectionBlueprints SelectionBlueprints { get; private set; }
    public SelectionHandler SelectionHandler { get; private set; }

    private InputManager inputManager;
    private MouseButtonEvent lastDragEvent;
    private readonly Dictionary<ITimedObject, SelectionBlueprint> blueprints = new();

    // movement
    private bool isDragging;
    private SelectionBlueprint[] dragBlueprints;
    private Vector2[] dragBlueprintsPositions;

    private PlacementBlueprint currentPlacementBlueprint;
    private Container placementBlueprintContainer;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        currentTool = ChartingContainer.Tools[0] as SelectTool;

        SelectionHandler = new SelectionHandler();
        SelectionHandler.SelectedObjects.BindTo(SelectedObjects);

        InternalChildren = new Drawable[]
        {
            SelectionBox = new SelectionBox { Playfield = ChartingContainer.Playfield },
            SelectionHandler,
            SelectionBlueprints = new SelectionBlueprints(),
            placementBlueprintContainer = new Container { RelativeSizeAxes = Axes.Both }
        };
    }

    protected override void LoadComplete()
    {
        inputManager = GetContainingInputManager();

        if (ChartingContainer == null) return;

        values.MapInfo.HitObjectAdded += AddBlueprint;
        values.MapInfo.HitObjectRemoved += RemoveBlueprint;

        values.MapEvents.FlashEventAdded += AddBlueprint;
        values.MapEvents.FlashEventRemoved += RemoveBlueprint;

        foreach (var hitObject in ChartingContainer.HitObjects)
            AddBlueprint(hitObject.Data);

        foreach (var flash in ChartingContainer.Playfield.Effects.Flashes)
            AddBlueprint(flash.FlashEvent);
    }

    protected override bool OnDragStart(DragStartEvent e)
    {
        if (e.Button != MouseButton.Left) return false;

        lastDragEvent = e;

        if (dragBlueprints != null)
        {
            isDragging = true;
            return true;
        }

        SelectionBox.HandleDrag(e);
        SelectionBox.Show();
        return true;
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        var foundByClick = selectByClick(e);
        var canMove = prepareMovement();
        return foundByClick || canMove;
    }

    protected override void OnDrag(DragEvent e)
    {
        lastDragEvent = e;

        if (isDragging)
            moveCurrentSelection(e);
    }

    protected override void OnMouseUp(MouseUpEvent e) => clearDragStuff();
    protected override void OnDragEnd(DragEndEvent e) => clearDragStuff();

    private void clearDragStuff()
    {
        lastDragEvent = null;
        isDragging = false;
        dragBlueprints = null;
        SelectionBox.Hide();
    }

    public void AddBlueprint(ITimedObject info)
    {
        if (blueprints.ContainsKey(info))
            return;

        var blueprint = createBlueprint(info);
        blueprints[info] = blueprint;
        blueprint.Selected += onSelected;
        blueprint.Deselected += onDeselected;
        SelectionBlueprints.Add(blueprint);
    }

    public void RemoveBlueprint(ITimedObject obj)
    {
        if (!blueprints.Remove(obj, out var blueprint))
            return;

        blueprint.Deselect();
        blueprint.Selected -= onSelected;
        blueprint.Deselected -= onDeselected;
        SelectionBlueprints.Remove(blueprint, true);
    }

    private SelectionBlueprint createBlueprint(ITimedObject obj)
    {
        SelectionBlueprint blueprint = null!;

        switch (obj)
        {
            case HitObject hit:
                var hitDrawable = ChartingContainer.HitObjects.FirstOrDefault(d => d.Data == obj);
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

    private bool selectByClick(MouseButtonEvent e)
    {
        foreach (SelectionBlueprint blueprint in SelectionBlueprints.AliveChildren.Reverse().OrderByDescending(b => b.IsSelected))
        {
            if (!blueprint.IsHovered) continue;

            return SelectionHandler.SingleClickSelection(blueprint, e);
        }

        return false;
    }

    protected override void Update()
    {
        base.Update();

        if (lastDragEvent != null && SelectionBox.State == Visibility.Visible)
        {
            lastDragEvent.Target = this;
            SelectionBox.HandleDrag(lastDragEvent);
            UpdateSelection();
        }

        if (currentPlacementBlueprint != null)
        {
            switch (currentPlacementBlueprint.State)
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
            ensurePlacementCreated();

        if (currentPlacementBlueprint != null)
            updatePlacementPosition();
    }

    private void ensurePlacementCreated()
    {
        if (currentPlacementBlueprint != null) return;

        var blueprint = CurrentTool?.CreateBlueprint();

        if (blueprint != null)
        {
            placementBlueprintContainer.Child = currentPlacementBlueprint = blueprint;
            // updatePlacementPosition();
        }
    }

    private void updatePlacementPosition()
    {
        var hitObjectContainer = ChartingContainer.Playfield.HitObjectContainer;
        var mousePosition = inputManager.CurrentState.Mouse.Position;

        var time = hitObjectContainer.SnapTime(hitObjectContainer.TimeAtScreenSpacePosition(mousePosition));
        var lane = hitObjectContainer.LaneAtScreenSpacePosition(mousePosition);
        currentPlacementBlueprint.UpdatePlacement(time, lane);
    }

    private void removePlacement()
    {
        currentPlacementBlueprint?.EndPlacement(false);
        currentPlacementBlueprint?.Expire();
        currentPlacementBlueprint = null;
    }

    public void UpdateSelection()
    {
        var quad = SelectionBox.Box.ScreenSpaceDrawQuad;

        foreach (var blueprint in SelectionBlueprints)
        {
            switch (blueprint.State)
            {
                case SelectedState.Selected:
                    if (!quad.Contains(blueprint.ScreenSpaceSelectionPoint))
                        blueprint.Deselect();
                    break;

                case SelectedState.Deselected:
                    if (blueprint.IsAlive && blueprint.IsPresent && quad.Contains(blueprint.ScreenSpaceSelectionPoint))
                        blueprint.Select();
                    break;
            }
        }
    }

    public void SelectAll()
    {
        foreach (var blueprint in SelectionBlueprints)
            blueprint.Select();
    }

    private void onSelected(SelectionBlueprint blueprint)
    {
        SelectionHandler.HandleSelection(blueprint);
    }

    private void onDeselected(SelectionBlueprint blueprint)
    {
        SelectionHandler.HandleDeselection(blueprint);
    }

    private bool prepareMovement()
    {
        if (!SelectionHandler.Selected.Any())
            return false;

        if (!SelectionHandler.Selected.Any(b => b.IsHovered))
            return false;

        dragBlueprints = SelectionHandler.Selected.ToArray();
        dragBlueprintsPositions = dragBlueprints.Select(m => m.ScreenSpaceSelectionPoint).ToArray();
        return true;
    }

    private void moveCurrentSelection(DragEvent e)
    {
        if (dragBlueprints == null) return;

        Vector2 delta = e.ScreenSpaceMousePosition - e.ScreenSpaceMouseDownPosition;

        Vector2 postition = dragBlueprintsPositions.First() + delta;
        float time = ChartingContainer.Playfield.HitObjectContainer.TimeAtScreenSpacePosition(postition);
        int lane = ChartingContainer.Playfield.HitObjectContainer.LaneAtScreenSpacePosition(postition);
        float snappedTime = ChartingContainer.Playfield.HitObjectContainer.SnapTime(time);

        float timeDelta = snappedTime - dragBlueprints.First().Object.Time;
        int laneDelta = 0;

        if (dragBlueprints.Any(x => x is NoteSelectionBlueprint))
        {
            var hitBlueprints = dragBlueprints.OfType<NoteSelectionBlueprint>().ToArray();

            laneDelta = lane - hitBlueprints.First().Object.Lane;

            var minLane = hitBlueprints.Min(b => b.Object.Lane);
            var maxLane = hitBlueprints.Max(b => b.Object.Lane);

            if (minLane + laneDelta <= 0 || maxLane + laneDelta > values.Editor.Map.KeyCount)
                laneDelta = 0;
        }

        foreach (var blueprint in dragBlueprints)
        {
            blueprint.Object.Time += timeDelta;

            if (blueprint is NoteSelectionBlueprint noteBlueprint)
                noteBlueprint.Object.Lane += laneDelta;
        }
    }
}

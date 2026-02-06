using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Overlay.Mouse;
using fluXis.Screens.Edit.Blueprints.Selection;
using fluXis.Screens.Edit.Input;
using fluXis.UI;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace fluXis.Screens.Edit.Blueprints;

public partial class BlueprintContainer<T> : Container, ICursorDrag, IKeyBindingHandler<EditorKeybinding>
    where T : class
{
    protected virtual bool HorizontalSelection => false;
    protected virtual bool InArea => false;
    protected virtual bool OnlySelectOnDrag => false;

    protected readonly BindableList<T> SelectedObjects = new();

    protected SelectionBox SelectionBox { get; private set; }
    protected SelectionBlueprints<T> SelectionBlueprints { get; private set; }
    public SelectionHandler<T> SelectionHandler { get; private set; }

    protected InputManager InputManager { get; private set; }

    private MouseButtonEvent lastDragEvent;
    private readonly Dictionary<T, SelectionBlueprint<T>> blueprints = new();

    // movement
    private bool isDraggingBlueprints;
    protected SelectionBlueprint<T>[] DraggedBlueprints { get; set; }
    protected Vector2[] DraggedBlueprintsPositions { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;

        SelectionHandler = CreateSelectionHandler();
        SelectionHandler.SelectedObjects.BindTo(SelectedObjects);

        if (OnlySelectOnDrag)
            SelectionHandler.HighlightPredicate = () => SelectedObjects?.Count > 1 || (SelectedObjects?.Count > 0 && IsDragged);

        InternalChildren = new Drawable[]
        {
            SelectionBox = new SelectionBox(HorizontalSelection),
            SelectionHandler,
            SelectionBlueprints = new SelectionBlueprints<T>()
        };
    }

    protected virtual SelectionHandler<T> CreateSelectionHandler() => new();

    protected override void LoadComplete()
    {
        InputManager = GetContainingInputManager();
    }

    public bool OnPressed(KeyBindingPressEvent<EditorKeybinding> e)
    {
        if (!SelectionHandler.Selected.Any())
            return false;

        if (!IsHovered)
            return false;

        switch (e.Action)
        {
            case EditorKeybinding.DeleteSelection:
                DeleteSelection();
                return true;

            case EditorKeybinding.CloneSelection:
                CloneSelection();
                return true;
        }

        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<EditorKeybinding> e) { }

    protected override bool OnDragStart(DragStartEvent e)
    {
        if (e.Button != MouseButton.Left) return false;

        lastDragEvent = e;

        if (DraggedBlueprints != null)
        {
            isDraggingBlueprints = true;
            StartedMoving();
            return true;
        }

        SelectionBox.HandleDrag(e);
        SelectionBox.Show();
        return true;
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        var foundByClick = selectByClick(e);
        var canMove = prepareMovement(e);

        var handle = foundByClick || canMove;

        if (!handle && !InArea)
            DeselectAll();

        return OnlySelectOnDrag ? base.OnMouseDown(e) : handle;
    }

    protected override void OnDrag(DragEvent e)
    {
        lastDragEvent = e;

        if (isDraggingBlueprints)
            MoveSelection(e);
    }

    protected override void OnMouseUp(MouseUpEvent e) => clearDragStuff();
    protected override void OnDragEnd(DragEndEvent e) => clearDragStuff();

    private void clearDragStuff()
    {
        if (isDraggingBlueprints)
            FinishedMoving();

        lastDragEvent = null;
        isDraggingBlueprints = false;
        DraggedBlueprints = null;
        SelectionBox.Hide();
    }

    public void AddBlueprint(T info)
    {
        addBlueprintExtra(info);
    }

    public void AddBlueprint(T info, params object[] extra)
    {
        addBlueprintExtra(info, extra);
    }

    private void addBlueprintExtra(T info, params object[] extra)
    {
        if (blueprints.ContainsKey(info))
            return;

        var blueprint = CreateBlueprintExtra(info, extra);

        if (blueprint == null)
            return;

        blueprints[info] = blueprint;
        blueprint.Selected += onSelected;
        blueprint.Deselected += onDeselected;
        SelectionBlueprints.Add(blueprint);
    }

    public void RemoveBlueprint(T obj)
    {
        removeBlueprintExtra(obj);
    }

    public void RemoveBlueprint(T obj, params object[] extra)
    {
        removeBlueprintExtra(obj, extra);
    }

    private void removeBlueprintExtra(T obj, params object[] extra)
    {
        if (!blueprints.Remove(obj, out var blueprint))
            return;

        blueprint.Deselect();
        blueprint.Selected -= onSelected;
        blueprint.Deselected -= onDeselected;
        SelectionBlueprints.Remove(blueprint, true);

        OnBlueprintRemoved(obj, extra);
    }

    public void RemoveAllBlueprints()
    {
        var blueprints = this.blueprints.Keys.ToList();
        blueprints.ForEach(RemoveBlueprint);
    }

    protected virtual void OnBlueprintRemoved(T obj, params object[] extra) { }

    protected virtual SelectionBlueprint<T> CreateBlueprintExtra(T obj, params object[] extra)
    {
        if (extra != null && extra.Length > 0)
        {
            var blueprint = CreateBlueprint(obj, extra);
            if (blueprint != null)
                return blueprint;
        }

        return CreateBlueprint(obj);
    }

    protected virtual SelectionBlueprint<T> CreateBlueprint(T obj) => null!;

    protected virtual SelectionBlueprint<T> CreateBlueprint(T obj, params object[] extra) => null!;

    private bool selectByClick(MouseButtonEvent e)
    {
        foreach (var blueprint in SelectionBlueprints.All.Reverse().OrderByDescending(b => b.IsSelected))
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
    }

    public void UpdateSelection()
    {
        var quad = SelectionBox.Box.ScreenSpaceDrawQuad;

        foreach (var blueprint in SelectionBlueprints.All)
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

    public void Select(T obj)
    {
        if (blueprints.TryGetValue(obj, out var blueprint))
            blueprint.Select();
    }

    public void SelectAll()
        => SelectionHandler.HandleSelection(SelectionBlueprints.All);

    public void DeselectAll() => SelectionHandler.DeselectAll();

    private void onSelected(SelectionBlueprint<T> blueprint) => SelectionHandler.HandleSelection(blueprint);
    private void onDeselected(SelectionBlueprint<T> blueprint) => SelectionHandler.HandleDeselection(blueprint);

    private bool prepareMovement(MouseDownEvent e)
    {
        if (e.Button != MouseButton.Left)
            return false;

        if (!SelectionHandler.Selected.Any())
            return false;

        if (!SelectionHandler.Selected.Any(b => b.IsHovered))
            return false;

        DraggedBlueprints = SelectionHandler.Selected.ToArray();
        DraggedBlueprintsPositions = DraggedBlueprints.Select(m => m.ScreenSpaceSelectionPoint).ToArray();
        return true;
    }

    protected virtual void StartedMoving() { }
    protected virtual void MoveSelection(DragEvent e) { }
    public virtual void CloneSelection() { }
    public virtual void DeleteSelection() => SelectionHandler.DeleteSelected();
    protected virtual void FinishedMoving() { }
}

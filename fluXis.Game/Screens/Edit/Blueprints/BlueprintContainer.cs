using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Overlay.Mouse;
using fluXis.Game.Screens.Edit.Blueprints.Selection;
using fluXis.Game.UI;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace fluXis.Game.Screens.Edit.Blueprints;

public partial class BlueprintContainer<T> : Container, ICursorDrag
{
    protected virtual bool HorizontalSelection => false;
    protected virtual bool InArea => false;

    protected readonly BindableList<T> SelectedObjects = new();

    protected SelectionBox SelectionBox { get; private set; }
    protected SelectionBlueprints<T> SelectionBlueprints { get; private set; }
    public SelectionHandler<T> SelectionHandler { get; private set; }

    protected InputManager InputManager { get; private set; }

    private MouseButtonEvent lastDragEvent;
    private readonly Dictionary<T, SelectionBlueprint<T>> blueprints = new();

    // movement
    private bool isDragging;
    protected SelectionBlueprint<T>[] DraggedBlueprints { get; set; }
    protected Vector2[] DraggedBlueprintsPositions { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;

        SelectionHandler = CreateSelectionHandler();
        SelectionHandler.SelectedObjects.BindTo(SelectedObjects);

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

    protected override bool OnDragStart(DragStartEvent e)
    {
        if (e.Button != MouseButton.Left) return false;

        lastDragEvent = e;

        if (DraggedBlueprints != null)
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
        var canMove = prepareMovement(e);

        var handle = foundByClick || canMove;

        if (!handle && !InArea)
            SelectionHandler.DeselectAll();

        return handle;
    }

    protected override void OnDrag(DragEvent e)
    {
        lastDragEvent = e;

        if (isDragging)
            MoveSelection(e);
    }

    protected override void OnMouseUp(MouseUpEvent e) => clearDragStuff();
    protected override void OnDragEnd(DragEndEvent e) => clearDragStuff();

    private void clearDragStuff()
    {
        lastDragEvent = null;
        isDragging = false;
        DraggedBlueprints = null;
        SelectionBox.Hide();
    }

    public void AddBlueprint(T info)
    {
        if (blueprints.ContainsKey(info))
            return;

        var blueprint = CreateBlueprint(info);
        blueprints[info] = blueprint;
        blueprint.Selected += onSelected;
        blueprint.Deselected += onDeselected;
        SelectionBlueprints.Add(blueprint);
    }

    public void RemoveBlueprint(T obj)
    {
        if (!blueprints.Remove(obj, out var blueprint))
            return;

        blueprint.Deselect();
        blueprint.Selected -= onSelected;
        blueprint.Deselected -= onDeselected;
        SelectionBlueprints.Remove(blueprint, true);
    }

    protected virtual SelectionBlueprint<T> CreateBlueprint(T obj) => null!;

    private bool selectByClick(MouseButtonEvent e)
    {
        foreach (var blueprint in SelectionBlueprints.AliveChildren.Reverse().OrderByDescending(b => b.IsSelected))
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

    protected virtual void MoveSelection(DragEvent e) { }
}

using System.Linq;
using fluXis.Game.Map;
using fluXis.Game.Screens.Edit.Tabs.Charting.Selection;
using fluXis.Game.UI;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Blueprints;

public partial class BlueprintContainer : Container
{
    public ChartingContainer ChartingContainer { get; init; }

    protected readonly BindableList<HitObjectInfo> SelectedHitObjects = new();

    public SelectionBox SelectionBox { get; private set; }
    public SelectionBlueprints SelectionBlueprints { get; private set; }
    public SelectionHandler SelectionHandler { get; private set; }

    private MouseButtonEvent lastDragEvent;

    // movement
    private bool isDragging;
    private SelectionBlueprint[] dragBlueprints;
    private Vector2[] dragBlueprintsPositions;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;

        SelectionHandler = new SelectionHandler();
        SelectionHandler.SelectedHitObjects.BindTo(SelectedHitObjects);

        InternalChildren = new Drawable[]
        {
            SelectionBox = new SelectionBox { Playfield = ChartingContainer.Playfield },
            SelectionHandler,
            SelectionBlueprints = new SelectionBlueprints()
        };
    }

    protected override void LoadComplete()
    {
        if (ChartingContainer == null) return;

        foreach (var hitObject in ChartingContainer.HitObjects)
            AddBlueprint(hitObject.Data);
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
        return e.Button == MouseButton.Left && prepareMovement();
    }

    protected override void OnDrag(DragEvent e)
    {
        lastDragEvent = e;

        if (isDragging)
            moveCurrentSelection(e);
    }

    protected override void OnDragEnd(DragEndEvent e)
    {
        lastDragEvent = null;
        isDragging = false;
        dragBlueprints = null;
        SelectionBox.Hide();
    }

    public void AddBlueprint(HitObjectInfo info)
    {
        var blueprint = createBlueprint(info);
        blueprint.Selected += onSelected;
        blueprint.Deselected += onDeselected;
        SelectionBlueprints.Add(blueprint);
    }

    private SelectionBlueprint createBlueprint(HitObjectInfo info)
    {
        var drawable = ChartingContainer.HitObjects.FirstOrDefault(d => d.Data == info);

        if (drawable == null) return null;

        SelectionBlueprint blueprint = info.IsLongNote() ? new LongNoteSelectionBlueprint(info) : new SingleNoteSelectionBlueprint(info);
        blueprint.Drawable = drawable;
        return blueprint;
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

        float timeDelta = snappedTime - dragBlueprints.First().HitObject.Time;
        int laneDelta = lane - dragBlueprints.First().HitObject.Lane;

        foreach (var blueprint in dragBlueprints)
        {
            blueprint.HitObject.Time += timeDelta;
            blueprint.HitObject.Lane += laneDelta;
        }
    }
}

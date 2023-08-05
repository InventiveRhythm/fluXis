using fluXis.Game.Map;
using fluXis.Game.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osuTK.Input;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Blueprints;

public partial class BlueprintContainer : Container
{
    public SelectionBox SelectionBox { get; private set; }
    public SelectionBlueprints SelectionBlueprints { get; private set; }

    private MouseButtonEvent lastDragEvent;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        InternalChildren = new Drawable[]
        {
            SelectionBox = new SelectionBox(),
            SelectionBlueprints = new SelectionBlueprints()
        };
    }

    protected override bool OnDragStart(DragStartEvent e)
    {
        if (e.Button != MouseButton.Left) return false;

        lastDragEvent = e;

        SelectionBox.HandleDrag(e);
        SelectionBox.Show();
        return true;
    }

    protected override void OnDrag(DragEvent e)
    {
        lastDragEvent = e;
    }

    protected override void OnDragEnd(DragEndEvent e)
    {
        lastDragEvent = null;
        SelectionBox.Hide();
    }

    public void AddBlueprint(HitObjectInfo info)
    {
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
}

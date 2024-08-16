using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osuTK.Input;

namespace fluXis.Game.Screens.Edit.Blueprints.Selection;

public partial class SelectionHandler<T> : Container, IHasContextMenu
{
    public virtual MenuItem[] ContextMenuItems => Array.Empty<MenuItem>();

    public IReadOnlyList<SelectionBlueprint<T>> Selected => selected;
    private readonly List<SelectionBlueprint<T>> selected = new();

    public readonly BindableList<T> SelectedObjects = new();

    private SelectionOutline outline;
    private bool wasVisible;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        AlwaysPresent = true;
        InternalChild = outline = new SelectionOutline();

        SelectedObjects.BindCollectionChanged((_, _) => updateVisibility());
    }

    protected override void Update()
    {
        base.Update();

        if (selected.Count == 0)
            return;

        RectangleF rect = ToLocalSpace(selected[0].Drawable.ScreenSpaceDrawQuad).AABBFloat;

        for (int i = 1; i < selected.Count; i++)
            rect = RectangleF.Union(rect, ToLocalSpace(selected[i].Drawable.ScreenSpaceDrawQuad).AABBFloat);

        rect = rect.Inflate(5);
        outline.UpdatePositionAndSize(rect.Location, rect.Size, !wasVisible);
        wasVisible = true;
    }

    public void HandleSelection(SelectionBlueprint<T> blueprint)
    {
        if (!SelectedObjects.Contains(blueprint.Object))
            SelectedObjects.Add(blueprint.Object);

        selected.Add(blueprint);
    }

    public void HandleDeselection(SelectionBlueprint<T> blueprint)
    {
        SelectedObjects.Remove(blueprint.Object);
        selected.Remove(blueprint);
    }

    private void updateVisibility()
    {
        outline.FadeTo(SelectedObjects.Count > 0 ? 1 : 0);

        if (SelectedObjects.Count == 0) wasVisible = false;
    }

    public bool SingleClickSelection(SelectionBlueprint<T> blueprint, MouseButtonEvent e)
    {
        switch (e.Button)
        {
            case MouseButton.Left:
                if (e.ControlPressed)
                {
                    if (blueprint.IsSelected)
                        blueprint.Deselect();
                    else
                        blueprint.Select();

                    return true;
                }

                if (blueprint.IsSelected) return false;

                DeselectAll();
                blueprint.Select();
                return true;

            case MouseButton.Right:
                quickDelete(blueprint);
                return true;

            default:
                return false;
        }
    }

    private void quickDelete(SelectionBlueprint<T> blueprint)
    {
        if (blueprint == null) return;

        if (!blueprint.IsSelected)
            Delete(new[] { blueprint.Object });
        else
            DeleteSelected();
    }

    public virtual void Delete(IEnumerable<T> objects)
    {
    }

    public void DeleteSelected()
    {
        Delete(SelectedObjects.ToArray());
        DeselectAll();
    }

    public void DeselectAll()
    {
        selected.ToList().ForEach(b => b.Deselect());
        SelectedObjects.Clear();
    }
}

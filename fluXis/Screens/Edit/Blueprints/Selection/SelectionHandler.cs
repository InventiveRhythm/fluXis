using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Input;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osuTK.Input;

namespace fluXis.Screens.Edit.Blueprints.Selection;

public partial class SelectionHandler<T> : Container, IKeyBindingHandler<FluXisGlobalKeybind>
{
    public IReadOnlyList<SelectionBlueprint<T>> Selected => selected;
    private readonly List<SelectionBlueprint<T>> selected = new();

    public readonly BindableList<T> SelectedObjects = new();

    public Func<bool> HighlightPredicate;

    private SelectionOutline outline;
    private bool wasVisible;

    public SelectionHandler()
    {
        HighlightPredicate = () => SelectedObjects.Count > 0;
    }

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

        RectangleF rect = ToLocalSpace(selected[0].ScreenSpaceDrawQuad).AABBFloat;

        for (int i = 1; i < selected.Count; i++)
        {
            var blueprint = selected[i];
            rect = RectangleF.Union(rect, ToLocalSpace(blueprint.ScreenSpaceDrawQuad).AABBFloat);
        }

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

    public void HandleSelection(IEnumerable<SelectionBlueprint<T>> blueprints)
    {
        var list = blueprints.ToList();

        var objects = list.Where(x => !SelectedObjects.Contains(x.Object)).Select(x => x.Object);
        SelectedObjects.AddRange(objects);

        selected.AddRange(list);
        list.ForEach(l => l.Select());
    }

    public void HandleDeselection(SelectionBlueprint<T> blueprint)
    {
        if (SelectedObjects.Contains(blueprint.Object))
            SelectedObjects.Remove(blueprint.Object);

        if (selected.Contains(blueprint))
            selected.Remove(blueprint);
    }

    private void updateVisibility()
    {
        outline.FadeTo(HighlightPredicate() ? 1 : 0, 100);

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
        var list = selected.ToList();

        selected.Clear();
        SelectedObjects.Clear();

        list.ForEach(b => b.Deselect());
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        if (e.Action != FluXisGlobalKeybind.Back || selected.Count <= 0)
            return false;

        DeselectAll();
        return true;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisGlobalKeybind> e)
    {
    }
}

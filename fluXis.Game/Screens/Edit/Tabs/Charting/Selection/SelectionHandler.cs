using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Map;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osuTK.Input;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Selection;

public partial class SelectionHandler : Container, IHasContextMenu
{
    public MenuItem[] ContextMenuItems => Array.Empty<MenuItem>();

    [Resolved]
    private EditorValues values { get; set; }

    public IReadOnlyList<SelectionBlueprint> Selected => selected;
    private readonly List<SelectionBlueprint> selected = new();

    public readonly BindableList<HitObjectInfo> SelectedHitObjects = new();

    private SelectionOutline outline;
    private bool wasVisible;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        AlwaysPresent = true;
        InternalChild = outline = new SelectionOutline();

        SelectedHitObjects.BindCollectionChanged((_, _) => updateVisibility());
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

    public void HandleSelection(SelectionBlueprint blueprint)
    {
        if (!SelectedHitObjects.Contains(blueprint.HitObject))
            SelectedHitObjects.Add(blueprint.HitObject);

        selected.Add(blueprint);
    }

    public void HandleDeselection(SelectionBlueprint blueprint)
    {
        SelectedHitObjects.Remove(blueprint.HitObject);
        selected.Remove(blueprint);
    }

    private void updateVisibility()
    {
        outline.FadeTo(SelectedHitObjects.Count > 0 ? 1 : 0);

        if (SelectedHitObjects.Count == 0) wasVisible = false;
    }

    public bool SingleClickSelection(SelectionBlueprint blueprint, MouseButtonEvent e)
    {
        if (e.Button == MouseButton.Right)
        {
            quickDelete(blueprint);
            return true;
        }

        return false;
    }

    private void quickDelete(SelectionBlueprint blueprint)
    {
        if (blueprint == null) return;

        if (!blueprint.IsSelected)
            Delete(new[] { blueprint.HitObject });
        else
            DeleteSelected();
    }

    public void Delete(IEnumerable<HitObjectInfo> hitObjects)
    {
        foreach (HitObjectInfo hitObject in hitObjects)
            values.MapInfo.Remove(hitObject);
    }

    public void DeleteSelected()
    {
        Delete(SelectedHitObjects.ToArray());
        DeselectAll();
    }

    public void DeselectAll()
    {
        SelectedHitObjects.Clear();
    }
}

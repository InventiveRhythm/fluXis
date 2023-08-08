using System;
using System.Collections.Generic;
using fluXis.Game.Map;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.UserInterface;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Selection;

public partial class SelectionHandler : Container, IHasContextMenu
{
    public MenuItem[] ContextMenuItems => Array.Empty<MenuItem>();

    public IReadOnlyList<SelectionBlueprint> Selected => selected;
    private readonly List<SelectionBlueprint> selected = new();

    public readonly BindableList<HitObjectInfo> SelectedHitObjects = new();

    private SelectionOutline outline;

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

        outline.Position = rect.Location;
        outline.Size = rect.Size;
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
    }
}

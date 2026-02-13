using System;
using System.Collections.Generic;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.UserInterface.Menus;
using fluXis.Graphics.UserInterface.Menus.Items;
using fluXis.Screens.Edit.Blueprints.Selection;
using fluXis.Storyboards;
using osu.Framework.Allocation;
using osu.Framework.Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.UserInterface;
using osuTK;

namespace fluXis.Screens.Edit.Tabs.Storyboarding.Animations.Blueprints;

public partial class StoryboardAnimationBlueprint : SelectionBlueprint<StoryboardAnimation>, IHasContextMenu
{
    [Resolved]
    private StoryboardAnimationsList animationList { get; set; }
    private StoryboardAnimationBlueprintContainer blueprints => animationList.Blueprints;

    [Resolved]
    private EditorSnapProvider snaps { get; set; }

    [Resolved]
    private Storyboard storyboard { get; set; }

    private StoryboardAnimationRow row => Drawable.Row;
    public new StoryboardAnimationEntry Drawable { get; internal set; } // hiding base member so we don't cast from Drawable every time

    public MenuItem[] ContextMenuItems => new List<MenuItem>
    {
        new MenuActionItem("Clone", FontAwesome6.Solid.Clone, MenuItemType.Normal, clone),
        new MenuActionItem("Edit", FontAwesome6.Solid.Pencil, MenuItemType.Normal, edit),
        new MenuActionItem("Delete", FontAwesome6.Solid.Trash, MenuItemType.Dangerous, delete),
    }.ToArray();

    public override double FirstComparer => Object.StartTime;
    public override double SecondComparer => Object.EndTime;

    public override Vector2 ScreenSpaceSelectionPoint => Drawable.ScreenSpaceDrawQuad.TopLeft;

    public StoryboardAnimationBlueprint(StoryboardAnimationEntry anim)
        : base(anim.Animation)
    {
        Drawable = anim;

        Height = 15f;
        Anchor = Origin = Anchor.CentreLeft;
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        Selected += _ => Drawable.IsSelected.Value = true;
        Deselected += _ => Drawable.IsSelected.Value = false;
    }

    protected override void Update()
    {
        base.Update();

        if (Parent == null)
            return;

        var rowIndex = animationList.GetRowIndex(row);
        var startX = animationList.PositionAtTime(Object.StartTime);
        var endX = animationList.PositionAtTime(Object.StartTime + Object.Duration);

        var row_height = StoryboardAnimationsList.ROW_HEIGHT;
        var diamondSize = 24f;

        Position = new Vector2(startX - diamondSize/4 - diamondSize/6, (rowIndex * row_height) + row_height + diamondSize/2);
        Width = Math.Max(StoryboardAnimationsList.ROW_HEIGHT - diamondSize/6, endX - startX + diamondSize/2);
    }

    private void clone()
    {
        if (blueprints.SelectionHandler.SelectedObjects.Count > 0)
            blueprints.CloneSelection();
        else
            animationList.CloneAnimation(Object, row);
    }

    private void edit()
    {
        Drawable.ShowPopover();
        Drawable.IsSelected.Value = true;
    }

    private void delete()
    {
        if (blueprints.SelectionHandler.SelectedObjects.Count > 0)
            blueprints.DeleteSelection();
        else
            row.Remove(Object);
    }
}
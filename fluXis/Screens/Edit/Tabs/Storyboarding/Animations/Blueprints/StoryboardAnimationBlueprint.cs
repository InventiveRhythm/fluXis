using System;
using System.Collections.Generic;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.UserInterface.Menus;
using fluXis.Graphics.UserInterface.Menus.Items;
using fluXis.Screens.Edit.Blueprints.Selection;
using fluXis.Screens.Edit.Tabs.Storyboarding.Timeline.Elements;
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

    private StoryboardAnimationRow row;
    private StoryboardAnimationEntry drawable;

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
        row = anim.Row;

        Height = 15f;
        Anchor = Origin = Anchor.CentreLeft;
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        drawable = animationList.GetDrawable(Object);

        Selected += _ => drawable.IsSelected.Value = true;
        Deselected += _ => drawable.IsSelected.Value = false;
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
        blueprints.CloneSelection();
    }

    private void edit()
    {
        drawable.ShowPopover();
        drawable.IsSelected.Value = true;
    }

    private void delete()
    {
        row.Remove(Object);
        blueprints.DeleteSelection();
    }
}
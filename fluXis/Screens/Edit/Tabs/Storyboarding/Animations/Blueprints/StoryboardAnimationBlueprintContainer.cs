using System.Linq;
using fluXis.Screens.Edit.Blueprints;
using fluXis.Screens.Edit.Blueprints.Selection;
using fluXis.Storyboards;
using osu.Framework.Allocation;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Input.Events;
using osuTK.Input;

namespace fluXis.Screens.Edit.Tabs.Storyboarding.Animations.Blueprints;

public partial class StoryboardAnimationBlueprintContainer : BlueprintContainer<StoryboardAnimation>
{
    protected override bool HorizontalSelection => true;
    protected override bool OnlyHighlightOnDrag => true;
    protected override bool BypassClickHandler => true;

    [Resolved]
    private EditorMap map { get; set; }

    [Resolved]
    private StoryboardAnimationsList animationList { get; set; }

    [Resolved]
    private EditorSnapProvider snaps { get; set; }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        animationList.FocusedElement += isSelected =>
        {
            if (isSelected)
            {
                animationList.AnimationAdded += onAnimationAdded;
                animationList.AnimationRemoved += onAnimationRemoved;

                animationList.AnimationsEnumerable.ForEach(anim => onAnimationAdded(anim));
            }
            else
            {
                RemoveAllBlueprints();
            }
        };
    }

    private void onAnimationAdded(StoryboardAnimationEntry anim) => AddBlueprint(anim.Animation, anim);

    private void onAnimationRemoved(StoryboardAnimationEntry anim) => RemoveBlueprint(anim.Animation, anim);

    protected override SelectionBlueprint<StoryboardAnimation> CreateBlueprint(StoryboardAnimation anim, params object[] extra)
    {
        var drawable = extra.Length > 0 ? (StoryboardAnimationEntry)extra[0]
            : animationList.GetDrawable(anim);

        var bp = new StoryboardAnimationBlueprint(drawable);
        return bp;
    }

    protected override bool OnMouseDown(MouseDownEvent e) => e.Button == MouseButton.Left && base.OnMouseDown(e);

    protected override void MoveSelection(DragEvent e)
    {
        if (DraggedBlueprints == null) return;

        var delta = e.ScreenSpaceMousePosition - e.ScreenSpaceMouseDownPosition;

        var position = DraggedBlueprintsPositions.First() + delta;
        var time = animationList.TimeAtScreenSpacePosition(position);
        var snappedTime = snaps.SnapTime(time, true);

        var timeDelta = snappedTime - DraggedBlueprints.First().Object.StartTime;

        foreach (var blueprint in DraggedBlueprints)
        {
            blueprint.Object.StartTime += timeDelta;
            map.Update(blueprint.Object);
        }
    }

    public override void CloneSelection()
    {
        var orderedBlueprints = SelectionHandler.Selected.OrderBy(b => b.Object.StartTime).ToList();

        if (orderedBlueprints.Count == 0) return;

        float firstStartTime = (float)orderedBlueprints[0].Object.StartTime;

        foreach (var blueprint in orderedBlueprints.Cast<StoryboardAnimationBlueprint>())
        {
            float normalizedTime = (float)blueprint.Object.StartTime - firstStartTime;
            animationList.CloneAnimation(blueprint.Object, blueprint.Drawable.Row, normalizedTime);
        }
    }

    public override void DeleteSelection()
    {
        var blueprintsToDelete = SelectionHandler.Selected.ToList();

        foreach (var blueprint in blueprintsToDelete.Cast<StoryboardAnimationBlueprint>())
        {
            var drawable = blueprint.Drawable;
            drawable.Row.Remove(blueprint.Object);
        }
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        if (animationList is not null)
        {
            animationList.AnimationAdded -= onAnimationAdded;
            animationList.AnimationRemoved -= onAnimationRemoved;
        }
    }
}

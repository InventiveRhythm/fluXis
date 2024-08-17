using fluXis.Game.Screens.Edit.Blueprints;
using fluXis.Game.Screens.Edit.Blueprints.Selection;
using fluXis.Game.Storyboards;
using osu.Framework.Allocation;

namespace fluXis.Game.Screens.Edit.Tabs.Storyboarding.Timeline.Blueprints;

public partial class TimelineBlueprintContainer : BlueprintContainer<StoryboardElement>
{
    protected override bool HorizontalSelection => true;

    [Resolved]
    private Storyboard storyboard { get; set; }

    [Resolved]
    private StoryboardTimeline timeline { get; set; }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        storyboard.ElementAdded += AddBlueprint;
        storyboard.ElementRemoved += RemoveBlueprint;
        storyboard.Elements.ForEach(AddBlueprint);
    }

    protected override SelectionBlueprint<StoryboardElement> CreateBlueprint(StoryboardElement element)
    {
        var bp = new TimelineElementBlueprint(element);
        bp.Drawable = timeline.GetDrawable(element);
        return bp;
    }
}

using fluXis.Map.Structures;
using fluXis.Map.Structures.Events;
using fluXis.Screens.Edit.Tabs.Charting.Playfield.Tags.TimingTags;

namespace fluXis.Screens.Edit.Tabs.Charting.Playfield.Tags;

public partial class TimingTagContainer : EditorTagContainer
{
    protected override void LoadComplete()
    {
        AddTag(new PreviewPointTag(this));

        foreach (var timingPoint in Map.MapInfo.TimingPoints)
            addTimingPoint(timingPoint);

        foreach (var sv in Map.MapInfo.ScrollVelocities)
            addScrollVelocity(sv);

        foreach (var sm in Map.MapInfo.MapEvents.ScrollMultiplyEvents)
            addScrollMultiplier(sm);

        Map.TimingPointAdded += addTimingPoint;
        Map.TimingPointRemoved += RemoveTag;
        Map.ScrollVelocityAdded += addScrollVelocity;
        Map.ScrollVelocityRemoved += RemoveTag;
        Map.ScrollMultiplierEventAdded += addScrollMultiplier;
        Map.ScrollMultiplierEventRemoved += RemoveTag;
    }

    private void addTimingPoint(TimingPoint tp) => AddTag(new TimingPointTag(this, tp));
    private void addScrollVelocity(ScrollVelocity sv) => AddTag(new ScrollVelocityTag(this, sv));
    private void addScrollMultiplier(ScrollMultiplierEvent sm) => AddTag(new ScrollMultiplierTag(this, sm));
}
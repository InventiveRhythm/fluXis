using fluXis.Game.Map.Structures;
using fluXis.Game.Screens.Edit.Tabs.Charting.Playfield.Tags.TimingTags;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Playfield.Tags;

public partial class TimingTagContainer : EditorTagContainer
{
    protected override void LoadComplete()
    {
        AddTag(new PreviewPointTag(this));

        foreach (var timingPoint in Map.MapInfo.TimingPoints)
            addTimingPoint(timingPoint);

        foreach (var sv in Map.MapInfo.ScrollVelocities)
            addScrollVelocity(sv);

        Map.TimingPointAdded += addTimingPoint;
        Map.TimingPointRemoved += RemoveTag;
        Map.ScrollVelocityAdded += addScrollVelocity;
        Map.ScrollVelocityRemoved += RemoveTag;
    }

    private void addTimingPoint(TimingPoint tp) => AddTag(new TimingPointTag(this, tp));
    private void addScrollVelocity(ScrollVelocity sv) => AddTag(new ScrollVelocityTag(this, sv));
}

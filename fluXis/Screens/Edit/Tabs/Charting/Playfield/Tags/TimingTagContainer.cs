using fluXis.Map.Structures;
using fluXis.Map.Structures.Events;
using fluXis.Map.Structures.Events.Scrolling;
using fluXis.Screens.Edit.Tabs.Charting.Playfield.Tags.EffectTags;
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

        Map.RegisterAddListener<TimingPoint>(addTimingPoint);
        Map.RegisterRemoveListener<TimingPoint>(RemoveTag);
        Map.RegisterAddListener<ScrollVelocity>(addScrollVelocity);
        Map.RegisterRemoveListener<ScrollVelocity>(RemoveTag);
        Map.RegisterAddListener<ScrollMultiplierEvent>(addScrollMultiplier);
        Map.RegisterRemoveListener<ScrollMultiplierEvent>(RemoveTag);

        Map.RegisterAddListener<NoteEvent>(addNote);
        Map.RegisterRemoveListener<NoteEvent>(RemoveTag);
        Map.MapEvents.NoteEvents.ForEach(addNote);
    }

    private void addTimingPoint(TimingPoint tp) => AddTag(new TimingPointTag(this, tp));
    private void addScrollVelocity(ScrollVelocity sv) => AddTag(new ScrollVelocityTag(this, sv));
    private void addScrollMultiplier(ScrollMultiplierEvent sm) => AddTag(new ScrollMultiplierTag(this, sm));

    private void addNote(NoteEvent note) => AddTag(new NoteEventTag(this, note));
}

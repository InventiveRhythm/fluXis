using System;
using System.Linq;
using fluXis.Map.Structures;
using fluXis.Map.Structures.Bases;
using fluXis.Map.Structures.Events;
using fluXis.Screens.Edit.UI.BottomBar.Timeline.Tags;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Screens.Edit.UI.BottomBar.Timeline;

public partial class TimelineTagContainer : Container
{
    [Resolved]
    private EditorClock clock { get; set; }

    [Resolved]
    private EditorMap map { get; set; }

    private Container<TimelineTimingPointTag> timingPoints;
    private Container<TimelineNoteTag> notePoints;
    // private Container chorusPoints;

    public float Offset
    {
        get => Y;
        set => Y = -value;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
        Height = 8;
        Y = Offset;

        Children = new Drawable[]
        {
            timingPoints = new() { RelativeSizeAxes = Axes.Both },
            notePoints = new() { RelativeSizeAxes = Axes.Both },
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        registerListeners(
            map.MapInfo.TimingPoints,
            timingPoints,
            tp => new TimelineTimingPointTag(clock, tp),
            t => (TimingPoint)t.TimedObject
        );

        registerListeners(
            map.MapInfo.MapEvents.NoteEvents,
            notePoints,
            n => new TimelineNoteTag(clock, n),
            t => (NoteEvent)t.TimedObject
        );
    }

    private void registerListeners<TObject, TTag>(
        System.Collections.Generic.List<TObject> items,
        Container<TTag> container,
        Func<TObject, TTag> f,
        Func<TTag, TObject> getTimedObject)
        where TObject : class, ITimedObject
        where TTag : Drawable
    {
        map.RegisterAddListener<TObject>(obj => addTag(container, f, obj));
        map.RegisterUpdateListener<TObject>(obj => updateTag(container, getTimedObject, obj));
        map.RegisterRemoveListener<TObject>(obj => removeTag(container, getTimedObject, obj));
        items.ForEach(obj => addTag(container, f, obj));
    }

    private void addTag<TObject, TTag>(Container<TTag> container, Func<TObject, TTag> f, TObject obj)
        where TObject : class, ITimedObject
        where TTag : Drawable
    {
        container.Add(createTag(f(obj), obj.Time));
    }

    private void updateTag<TObject, TTag>(Container<TTag> container, Func<TTag, TObject> getTimedObject, TObject obj)
        where TObject : class, ITimedObject
        where TTag : Drawable
    {
        var tag = container.FirstOrDefault(t => getTimedObject(t) == obj);
        if (tag != null)
            tag.X = calculatePosition(obj.Time);
    }

    private void removeTag<TObject, TTag>(Container<TTag> container, Func<TTag, TObject> getTimedObject, TObject obj)
        where TObject : class, ITimedObject
        where TTag : Drawable
    {
        var tag = container.FirstOrDefault(t => getTimedObject(t) == obj);
        if (tag != null)
            container.Remove(tag, true);
    }

    private T createTag<T>(T tag, double time) where T : Drawable
    {
        tag.X = calculatePosition(time);
        return tag;
    }

    private float calculatePosition(double time)
    {
        if (time == 0) return 0;
        var x = time / clock.TrackLength;
        return double.IsFinite(x) && !double.IsNaN(x) ? (float)x : 0;
    }
}

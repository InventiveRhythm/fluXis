using System;
using System.Linq;
using fluXis.Map.Structures;
using fluXis.Map.Structures.Bases;
using fluXis.Map.Structures.Events;
using fluXis.Screens.Edit.UI.BottomBar.Timeline.Tags;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

// ReSharper disable MissingBlankLines

namespace fluXis.Screens.Edit.UI.BottomBar.Timeline;

public partial class TimelineTagContainer : BufferedContainer
{
    [Resolved]
    private EditorClock clock { get; set; }

    [Resolved]
    private EditorMap map { get; set; }

    private Container<TimelineTimingPointTag> timingPoints;
    private Container<TimelineNoteTag> notePoints;
    // private Container chorusPoints;

    private float childLatestAnimEndTime;

    private const float width_scale = 1.1f;

    public float Offset
    {
        get => -Y;
        set => Y = -value;
    }

    public TimelineTagContainer()
        : base(cachedFrameBuffer: true)
    {
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
        Y = -Offset;

        // Height increased to accomodate for buffered containers getting clipped
        Height = 40;

        // children order is important for priorety - first: lowest, last: highest
        Children = new Drawable[]
        {
            notePoints = new() { RelativeSizeAxes = Axes.Both },
            timingPoints = new() { RelativeSizeAxes = Axes.Both },
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        // to prevent horizontal clipping
        Width *= width_scale;

        ForceRedraw();

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

    protected override void Update()
    {
        base.Update();

        if (Time.Current < childLatestAnimEndTime)
        {
            ForceRedraw();
        }
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        if (map == null)
            return;

        deRegisterListeners(
            map.MapInfo.TimingPoints,
            timingPoints,
            tp => new TimelineTimingPointTag(clock, tp),
            t => (TimingPoint)t.TimedObject
        );

        deRegisterListeners(
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
        where TTag : TimelineTag
    {
        map.RegisterAddListener<TObject>(obj => addTag(container, f, obj));
        map.RegisterUpdateListener<TObject>(obj => updateTag(container, getTimedObject, obj));
        map.RegisterRemoveListener<TObject>(obj => removeTag(container, getTimedObject, obj));
        items.ForEach(obj => addTag(container, f, obj));
    }

    private void deRegisterListeners<TObject, TTag>(
        System.Collections.Generic.List<TObject> items,
        Container<TTag> container,
        Func<TObject, TTag> f,
        Func<TTag, TObject> getTimedObject)
        where TObject : class, ITimedObject
        where TTag : TimelineTag
    {
        map.DeregisterAddListener<TObject>(obj => addTag(container, f, obj));
        map.DeregisterUpdateListener<TObject>(obj => updateTag(container, getTimedObject, obj));
        map.DeregisterRemoveListener<TObject>(obj => removeTag(container, getTimedObject, obj));
    }

    private void addTag<TObject, TTag>(Container<TTag> container, Func<TObject, TTag> f, TObject obj)
        where TObject : class, ITimedObject
        where TTag : TimelineTag
    {
        var tag = createTag(f(obj));
        container.Add(tag);
        tag.X = calculatePosition(obj.Time);
    }

    private void updateTag<TObject, TTag>(Container<TTag> container, Func<TTag, TObject> getTimedObject, TObject obj)
        where TObject : class, ITimedObject
        where TTag : TimelineTag
    {
        var tag = container.FirstOrDefault(t => getTimedObject(t) == obj);
        if (tag != null)
            tag.X = calculatePosition(obj.Time);
    }

    private void removeTag<TObject, TTag>(Container<TTag> container, Func<TTag, TObject> getTimedObject, TObject obj)
        where TObject : class, ITimedObject
        where TTag : TimelineTag
    {
        var tag = container.FirstOrDefault(t => getTimedObject(t) == obj);
        if (tag != null)
            container.Remove(tag, true);
    }

    private T createTag<T>(T tag) where T : TimelineTag
    {
        tag.AnimationEnd = t => childLatestAnimEndTime = t;
        tag.OnDeferredUpdate = ForceRedraw;
        return tag;
    }

    private float calculatePosition(double time)
    {
        if (time == 0) return 0;

        var p = time / clock.TrackLength;
        var x = (p + (width_scale - 1) / 2.0) / width_scale;
        return double.IsFinite(x) && !double.IsNaN(x) ? (float)x : 0;
    }
}

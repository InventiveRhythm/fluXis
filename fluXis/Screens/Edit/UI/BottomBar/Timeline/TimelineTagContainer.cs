using System.Linq;
using fluXis.Map.Structures;
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

    private void init()
    {
        foreach (var tp in map.MapInfo.TimingPoints)
            timingPoints.Add(createTag(new TimelineTimingPointTag(clock, tp), tp.Time));

        foreach (var note in map.MapInfo.MapEvents.NoteEvents)
            notePoints.Add(createTag(new TimelineNoteTag(clock, note), note.Time));
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        ScheduleAfterChildren(init);

        map.RegisterAddListener<TimingPoint>(updateTimingPoint);
        map.RegisterUpdateListener<TimingPoint>(updateTimingPoint);
        map.RegisterRemoveListener<TimingPoint>(updateTimingPoint);

        map.RegisterAddListener<NoteEvent>(updateNote);
        map.RegisterUpdateListener<NoteEvent>(updateNote);
        map.RegisterRemoveListener<NoteEvent>(updateNote);
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

    private void updateTimingPoint(TimingPoint tp)
    {
        var tag = timingPoints.FirstOrDefault(t => t.TimedObject == tp);
        if (tag != null)
            tag.X = calculatePosition(tp.Time);
    }

    private void updateNote(NoteEvent n)
    {
        var tag = notePoints.FirstOrDefault(t => t.TimedObject == n);
        if (tag != null)
            tag.X = calculatePosition(n.Time);
    }
}

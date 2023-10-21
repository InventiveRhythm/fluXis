using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Map;
using fluXis.Game.Screens.Edit.Tabs.Charting.Playfield.Tags.TagTypes;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Playfield.Tags;

public partial class EditorTagContainer : Container<EditorTag>
{
    [Resolved]
    private EditorValues values { get; set; }

    [Resolved]
    private EditorClock clock { get; set; }

    [Resolved]
    private EditorChangeHandler changeHandler { get; set; }

    private readonly List<EditorTag> tags = new();

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.X;
        RelativeSizeAxes = Axes.Y;
        Anchor = Anchor.CentreLeft;
        Origin = Anchor.CentreRight;
        X = -20;

        foreach (var timingPoint in values.MapInfo.TimingPoints)
            addTimingPoint(timingPoint);

        foreach (var sv in values.MapInfo.ScrollVelocities)
            addScrollVelocity(sv);

        values.MapInfo.TimingPointAdded += addTimingPoint;
        values.MapInfo.TimingPointRemoved += removeTag;
        values.MapInfo.ScrollVelocityAdded += addScrollVelocity;
        values.MapInfo.ScrollVelocityRemoved += removeTag;
    }

    private void removeTag(TimedObject obj)
    {
        var tag = tags.FirstOrDefault(t => t.TimedObject == obj);

        if (tag != null)
            tags.Remove(tag);
    }

    private void addTimingPoint(TimingPointInfo tp) => tags.Add(new TimingPointTag(this, tp));
    private void addScrollVelocity(ScrollVelocityInfo sv) => tags.Add(new ScrollVelocityTag(this, sv));

    protected override void Update()
    {
        base.Update();

        var tagsToHide = Children.Where(t => t.Y < -20 || t.Y > DrawHeight + 20).ToList();

        foreach (var tag in tagsToHide)
        {
            tags.Add(tag);
            Remove(tag, false);
        }

        var tagsToDisplay = tags.Where(t => t.TimedObject.Time < clock.CurrentTime + 1000 && t.TimedObject.Time > clock.CurrentTime - 1000).ToList();

        foreach (var tag in tagsToDisplay)
        {
            tags.Remove(tag);
            Add(tag);
        }

        Dictionary<int, int> tagsAtTime = new Dictionary<int, int>();

        foreach (var tag in Children)
        {
            var time = (int)tag.TimedObject.Time;

            if (tagsAtTime.ContainsKey(time))
            {
                tag.X = tagsAtTime[time] * -90;
                tagsAtTime[time]++;
            }
            else
            {
                tag.X = 0;
                tagsAtTime[time] = 1;
            }
        }
    }
}

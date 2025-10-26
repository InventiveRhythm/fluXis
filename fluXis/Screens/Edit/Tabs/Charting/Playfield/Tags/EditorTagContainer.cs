using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Map.Structures.Bases;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Utils;

namespace fluXis.Screens.Edit.Tabs.Charting.Playfield.Tags;

public partial class EditorTagContainer : Container<EditorTag>
{
    [Resolved]
    protected EditorSettings Settings { get; private set; }

    [Resolved]
    protected EditorMap Map { get; private set; }

    [Resolved]
    protected EditorClock EditorClock { get; private set; }

    protected List<EditorTag> Tags { get; } = new();
    protected virtual bool RightSide => false;
    
    private bool needsSort = false;
    private EditorTag[] sortedChildrenCache;

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.X;
        RelativeSizeAxes = Axes.Y;
        Anchor = RightSide ? Anchor.CentreRight : Anchor.CentreLeft;
        Origin = RightSide ? Anchor.CentreLeft : Anchor.CentreRight;
        X = RightSide ? 20 : -20;
    }

    protected void AddTag(EditorTag tag)
    {
        tag.RightSide = RightSide;
        Tags.Add(tag);
    }

    protected void RemoveTag(ITimedObject obj)
    {
        var tag = Tags.FirstOrDefault(t => t.TimedObject == obj);
        tag ??= Children.FirstOrDefault(t => t.TimedObject == obj);

        if (tag == null) return;

        Tags.Remove(tag);
        Remove(tag, true);
        needsSort = true;
    }

    protected override void Update()
    {
        base.Update();

        var tagsToHide = Children.Where(t => t.Y < -20 || t.Y > DrawHeight + 20).ToList();

        foreach (var tag in tagsToHide)
        {
            Tags.Add(tag);
            Remove(tag, false);
            needsSort = true;
        }

        var tagsToDisplay = Tags.Where(t => t.TimedObject.Time < EditorClock.CurrentTime + 1000 && t.TimedObject.Time > EditorClock.CurrentTime - 1000).ToList();

        foreach (var tag in tagsToDisplay)
        {
            Tags.Remove(tag);
            Add(tag);
            needsSort = true;
        }

        if (needsSort || sortedChildrenCache == null)
        {
            sortedChildrenCache = Children.OrderBy(tag => (int)tag.TimedObject.Time).ToArray();
            needsSort = false;
        }

        var tagsAtTime = new Dictionary<int, int>();
        var timeOffsets = new Dictionary<int, float>();

        double timeThreshold = 10 / (Settings.ZoomBindable.Value - 0.99);
        timeThreshold = Math.Clamp(timeThreshold, 5, 25);

        foreach (var tag in sortedChildrenCache)
        {
            var time = (int)tag.TimedObject.Time;
            int closestTime = -1;

            foreach (var existingTime in tagsAtTime.Keys)
            {
                if (Precision.AlmostEquals(time, existingTime, timeThreshold))
                {
                    closestTime = existingTime;
                    break;
                }
            }

            if (closestTime != -1)
            {
                tag.X = timeOffsets[closestTime] * (RightSide ? 1 : -1);
                timeOffsets[closestTime] += tag.DrawWidth + 10;
                tagsAtTime[closestTime]++;
            }
            else
            {
                tag.X = 0;
                timeOffsets[time] = tag.DrawWidth + 10;
                tagsAtTime[time] = 1;
            }
        }
    }
}

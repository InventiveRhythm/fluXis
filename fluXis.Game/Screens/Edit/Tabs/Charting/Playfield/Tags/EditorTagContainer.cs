using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Map;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Playfield.Tags;

public partial class EditorTagContainer : Container<EditorTag>
{
    [Resolved]
    protected EditorValues Values { get; private set; }

    [Resolved]
    protected EditorClock EditorClock { get; private set; }

    protected List<EditorTag> Tags { get; } = new();
    protected virtual bool RightSide => false;

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

    protected void RemoveTag(TimedObject obj)
    {
        var tag = Tags.FirstOrDefault(t => t.TimedObject == obj);
        tag ??= Children.FirstOrDefault(t => t.TimedObject == obj);

        if (tag == null) return;

        Tags.Remove(tag);
        Remove(tag, true);
    }

    protected override void Update()
    {
        base.Update();

        var tagsToHide = Children.Where(t => t.Y < -20 || t.Y > DrawHeight + 20).ToList();

        foreach (var tag in tagsToHide)
        {
            Tags.Add(tag);
            Remove(tag, false);
        }

        var tagsToDisplay = Tags.Where(t => t.TimedObject.Time < EditorClock.CurrentTime + 1000 && t.TimedObject.Time > EditorClock.CurrentTime - 1000).ToList();

        foreach (var tag in tagsToDisplay)
        {
            Tags.Remove(tag);
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

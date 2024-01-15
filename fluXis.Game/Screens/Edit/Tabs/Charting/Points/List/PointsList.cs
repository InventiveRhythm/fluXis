using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Map.Events;
using fluXis.Game.Map.Structures;
using fluXis.Game.Screens.Edit.Tabs.Charting.Points.List.Entries;
using osu.Framework.Allocation;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Testing;
using osuTK;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Points.List;

public partial class PointsList : Container
{
    [Resolved]
    private EditorValues values { get; set; }

    public Action<IEnumerable<Drawable>> ShowSettings { get; init; }

    private bool initialLoad = true;
    private FillFlowContainer flow;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;

        InternalChild = new FluXisScrollContainer
        {
            RelativeSizeAxes = Axes.Both,
            ScrollbarVisible = false,
            Child = flow = new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Direction = FillDirection.Vertical,
                Spacing = new Vector2(10),
                Padding = new MarginPadding(10)
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        values.MapInfo.TimingPointAdded += addPoint;
        values.MapInfo.TimingPointChanged += updatePoint;
        values.MapInfo.TimingPointRemoved += removePoint;
        values.MapInfo.TimingPoints.ForEach(addPoint);

        values.MapInfo.ScrollVelocityAdded += addPoint;
        values.MapInfo.ScrollVelocityChanged += updatePoint;
        values.MapInfo.ScrollVelocityRemoved += removePoint;
        values.MapInfo.ScrollVelocities.ForEach(addPoint);

        values.MapEvents.LaneSwitchEventAdded += addPoint;
        values.MapEvents.LaneSwitchEventChanged += updatePoint;
        values.MapEvents.LaneSwitchEventRemoved += removePoint;
        values.MapEvents.LaneSwitchEvents.ForEach(addPoint);

        initialLoad = false;
        sortPoints();
    }

    private void sortPoints()
    {
        flow.ChildrenOfType<PointListEntry>().OrderBy(e => e.Object.Time).ForEach(e => flow.SetLayoutPosition(e, e.Object.Time));
    }

    private void addPoint(TimedObject obj)
    {
        PointListEntry entry = obj switch
        {
            TimingPoint timing => new TimingPointEntry(timing),
            ScrollVelocity scroll => new ScrollVelocityEntry(scroll),
            LaneSwitchEvent lane => new LaneSwitchEntry(lane),
            _ => null
        };

        if (entry != null)
        {
            entry.ShowSettings = ShowSettings;
            flow.Add(entry);
        }

        if (!initialLoad)
            sortPoints();
    }

    private void updatePoint(TimedObject obj)
    {
        var entry = flow.ChildrenOfType<PointListEntry>().FirstOrDefault(e => e.Object == obj);
        entry?.UpdateValues();

        sortPoints();
    }

    private void removePoint(TimedObject obj)
    {
        var entry = flow.ChildrenOfType<PointListEntry>().FirstOrDefault(e => e.Object == obj);

        if (entry != null)
            flow.Remove(entry, true);
    }
}

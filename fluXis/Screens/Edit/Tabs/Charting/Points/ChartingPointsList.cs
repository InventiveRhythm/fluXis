using System.Collections.Generic;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Structures;
using fluXis.Map.Structures.Bases;
using fluXis.Map.Structures.Events;
using fluXis.Screens.Edit.Tabs.Charting.Points.Entries;
using fluXis.Screens.Edit.Tabs.Shared.Points.List;

namespace fluXis.Screens.Edit.Tabs.Charting.Points;

public partial class ChartingPointsList : PointsList
{
    protected override void RegisterEvents()
    {
        Map.TimingPointAdded += AddPoint;
        Map.TimingPointUpdated += UpdatePoint;
        Map.TimingPointRemoved += RemovePoint;
        Map.MapInfo.TimingPoints.ForEach(AddPoint);

        Map.ScrollVelocityAdded += AddPoint;
        Map.ScrollVelocityUpdated += UpdatePoint;
        Map.ScrollVelocityRemoved += RemovePoint;
        Map.MapInfo.ScrollVelocities.ForEach(AddPoint);

        Map.LaneSwitchEventAdded += AddPoint;
        Map.LaneSwitchEventUpdated += UpdatePoint;
        Map.LaneSwitchEventRemoved += RemovePoint;
        Map.MapEvents.LaneSwitchEvents.ForEach(AddPoint);
    }

    protected override PointListEntry CreateEntryFor(ITimedObject obj)
    {
        return obj switch
        {
            TimingPoint timing => new TimingPointEntry(timing),
            ScrollVelocity scroll => new ScrollVelocityEntry(scroll),
            LaneSwitchEvent lane => new LaneSwitchEntry(lane),
            _ => null
        };
    }

    protected override IEnumerable<DropdownEntry> CreateDropdownEntries() => new DropdownEntry[]
    {
        new("Timing Point", FluXisColors.TimingPoint, () => Create(new TimingPoint()), x => x is TimingPoint),
        new("Scroll Velocity", FluXisColors.ScrollVelocity, () => Create(new ScrollVelocity()), x => x is ScrollVelocity)
    };
}

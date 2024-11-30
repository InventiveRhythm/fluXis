using System.Collections.Generic;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Map.Structures;
using fluXis.Game.Map.Structures.Bases;
using fluXis.Game.Map.Structures.Events;
using fluXis.Game.Screens.Edit.Tabs.Charting.Points.Entries;
using fluXis.Game.Screens.Edit.Tabs.Shared.Points.List;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Points;

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

    protected override IEnumerable<AddButtonEntry> CreateAddEntries() => new AddButtonEntry[]
    {
        new("Timing Point", FluXisColors.TimingPoint, () => Create(new TimingPoint())),
        new("Scroll Velocity", FluXisColors.ScrollVelocity, () => Create(new ScrollVelocity()))
    };
}

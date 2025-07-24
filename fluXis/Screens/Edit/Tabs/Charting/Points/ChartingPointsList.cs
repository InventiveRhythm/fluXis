using System.Collections.Generic;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Structures;
using fluXis.Map.Structures.Bases;
using fluXis.Map.Structures.Events;
using fluXis.Screens.Edit.Tabs.Charting.Points.Entries;
using fluXis.Screens.Edit.Tabs.Design.Points.Entries;
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

        Map.LaneSwitchEventAdded += AddPoint;
        Map.LaneSwitchEventUpdated += UpdatePoint;
        Map.LaneSwitchEventRemoved += RemovePoint;
        Map.MapEvents.LaneSwitchEvents.ForEach(AddPoint);

        Map.NoteEventAdded += AddPoint;
        Map.NoteEventUpdated += UpdatePoint;
        Map.NoteEventRemoved += RemovePoint;
        Map.MapEvents.NoteEvents.ForEach(AddPoint);
    }

    protected override PointListEntry CreateEntryFor(ITimedObject obj)
    {
        return obj switch
        {
            TimingPoint timing => new TimingPointEntry(timing),
            LaneSwitchEvent lane => new LaneSwitchEntry(lane),
            NoteEvent note => new NoteEntry(note),
            _ => null
        };
    }

    protected override IEnumerable<DropdownEntry> CreateDropdownEntries() => new DropdownEntry[]
    {
        new("Timing Point", Theme.TimingPoint, () => Create(new TimingPoint()), x => x is TimingPoint),
        new("Lane Switch", Theme.LaneSwitch, () => Create(new LaneSwitchEvent { Count = Map.RealmMap.KeyCount }), x => x is LaneSwitchEvent),
        new("Note", Theme.Note, () => Create(new NoteEvent()), x => x is NoteEvent),
    };
}

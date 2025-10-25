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
        RegisterTypeEvents(Map.MapInfo.TimingPoints);
        RegisterTypeEvents(Map.MapEvents.LaneSwitchEvents);
        RegisterTypeEvents(Map.MapEvents.NoteEvents);
    }

    protected override PointListEntry CreateEntryFor(ITimedObject obj) => obj switch
    {
        TimingPoint timing => new TimingPointEntry(timing),
        LaneSwitchEvent lane => new LaneSwitchEntry(lane),
        NoteEvent note => new NoteEntry(note),
        _ => null
    };

    protected override IEnumerable<DropdownEntry> CreateDropdownEntries() => new[]
    {
        CreateDefaultDropdownEntry<TimingPoint>("Timing Point", Theme.TimingPoint),
        CreateDefaultDropdownEntry<LaneSwitchEvent>("Lane Switch", Theme.LaneSwitch, l => l.Count = Map.RealmMap.KeyCount),
        CreateDefaultDropdownEntry<NoteEvent>("Note", Theme.Note)
    };
}

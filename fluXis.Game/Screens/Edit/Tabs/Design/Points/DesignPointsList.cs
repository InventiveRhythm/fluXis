using System.Collections.Generic;
using fluXis.Game.Map.Events;
using fluXis.Game.Map.Structures;
using fluXis.Game.Screens.Edit.Tabs.Design.Points.Entries;
using fluXis.Game.Screens.Edit.Tabs.Shared.Points.List;

namespace fluXis.Game.Screens.Edit.Tabs.Design.Points;

public partial class DesignPointsList : PointsList
{
    protected override void RegisterEvents()
    {
        Map.FlashEventAdded += AddPoint;
        Map.FlashEventUpdated += UpdatePoint;
        Map.FlashEventRemoved += RemovePoint;
        Map.MapEvents.FlashEvents.ForEach(AddPoint);

        Map.ShakeEventAdded += AddPoint;
        Map.ShakeEventUpdated += UpdatePoint;
        Map.ShakeEventRemoved += RemovePoint;
        Map.MapEvents.ShakeEvents.ForEach(AddPoint);

        Map.PlayfieldFadeEventAdded += AddPoint;
        Map.PlayfieldFadeEventUpdated += UpdatePoint;
        Map.PlayfieldFadeEventRemoved += RemovePoint;
        Map.MapEvents.PlayfieldFadeEvents.ForEach(AddPoint);

        Map.PlayfieldMoveEventAdded += AddPoint;
        Map.PlayfieldMoveEventUpdated += UpdatePoint;
        Map.PlayfieldMoveEventRemoved += RemovePoint;
        Map.MapEvents.PlayfieldMoveEvents.ForEach(AddPoint);

        Map.PlayfieldScaleEventAdded += AddPoint;
        Map.PlayfieldScaleEventUpdated += UpdatePoint;
        Map.PlayfieldScaleEventRemoved += RemovePoint;
        Map.MapEvents.PlayfieldScaleEvents.ForEach(AddPoint);

        Map.ShaderEventAdded += AddPoint;
        Map.ShaderEventUpdated += UpdatePoint;
        Map.ShaderEventRemoved += RemovePoint;
        Map.MapEvents.ShaderEvents.ForEach(AddPoint);

        Map.BeatPulseEventAdded += AddPoint;
        Map.BeatPulseEventUpdated += UpdatePoint;
        Map.BeatPulseEventRemoved += RemovePoint;
        Map.MapEvents.BeatPulseEvents.ForEach(AddPoint);

        Map.PlayfieldRotateEventAdded += AddPoint;
        Map.PlayfieldRotateEventUpdated += UpdatePoint;
        Map.PlayfieldRotateEventRemoved += RemovePoint;
        Map.MapEvents.PlayfieldRotateEvents.ForEach(AddPoint);

        Map.NoteEventAdded += AddPoint;
        Map.NoteEventUpdated += UpdatePoint;
        Map.NoteEventRemoved += RemovePoint;
        Map.MapEvents.NoteEvents.ForEach(AddPoint);
    }

    protected override PointListEntry CreateEntryFor(ITimedObject obj)
    {
        return obj switch
        {
            FlashEvent flash => new FlashEntry(flash),
            ShakeEvent shake => new ShakeEntry(shake),
            PlayfieldMoveEvent move => new PlayfieldMoveEntry(move),
            PlayfieldFadeEvent fade => new PlayfieldFadeEntry(fade),
            PlayfieldScaleEvent scale => new PlayfieldScaleEntry(scale),
            BeatPulseEvent pulse => new BeatPulseEntry(pulse),
            PlayfieldRotateEvent rotate => new PlayfieldRotateEntry(rotate),
            ShaderEvent shader => new ShaderEntry(shader),
            NoteEvent note => new NoteEntry(note),
            _ => null
        };
    }

    // flash is covered by the menu bar
    // TODO: Redesign the "add" menu
    protected override IEnumerable<AddButtonEntry> CreateAddEntries()
    {
        var entries = new List<AddButtonEntry>
        {
            new("Flash", () => Create(new FlashEvent())),
            new("Shake", () => Create(new ShakeEvent())),
            new("Playfield Fade", () => Create(new PlayfieldFadeEvent())),
            new("Playfield Move", () => Create(new PlayfieldMoveEvent())),
            new("Playfield Scale", () => Create(new PlayfieldScaleEvent())),
            new("Beat Pulse", () => Create(new BeatPulseEvent())),
            new("Playfield Rotate", () => Create(new PlayfieldRotateEvent())),
            new("Note", () => Create(new NoteEvent()))
        };

        if (FluXisGameBase.IsDebug)
            entries.Add(new AddButtonEntry("Shader", () => Create(new ShaderEvent { ShaderName = "Bloom" })));

        return entries;
    }
}

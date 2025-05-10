using System.Collections.Generic;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Structures;
using fluXis.Map.Structures.Bases;
using fluXis.Map.Structures.Events;
using fluXis.Screens.Edit.Tabs.Design.Points.Entries;
using fluXis.Screens.Edit.Tabs.Shared.Points.List;

namespace fluXis.Screens.Edit.Tabs.Design.Points;

public partial class DesignPointsList : PointsList
{
    protected override void RegisterEvents()
    {
        Map.ScrollVelocityAdded += AddPoint;
        Map.ScrollVelocityUpdated += UpdatePoint;
        Map.ScrollVelocityRemoved += RemovePoint;
        Map.MapInfo.ScrollVelocities.ForEach(AddPoint);

        Map.FlashEventAdded += AddPoint;
        Map.FlashEventUpdated += UpdatePoint;
        Map.FlashEventRemoved += RemovePoint;
        Map.MapEvents.FlashEvents.ForEach(AddPoint);

        Map.PulseEventAdded += AddPoint;
        Map.PulseEventUpdated += UpdatePoint;
        Map.PulseEventRemoved += RemovePoint;
        Map.MapEvents.PulseEvents.ForEach(AddPoint);

        Map.ShakeEventAdded += AddPoint;
        Map.ShakeEventUpdated += UpdatePoint;
        Map.ShakeEventRemoved += RemovePoint;
        Map.MapEvents.ShakeEvents.ForEach(AddPoint);

        Map.PlayfieldMoveEventAdded += AddPoint;
        Map.PlayfieldMoveEventUpdated += UpdatePoint;
        Map.PlayfieldMoveEventRemoved += RemovePoint;
        Map.MapEvents.PlayfieldMoveEvents.ForEach(AddPoint);

        Map.PlayfieldScaleEventAdded += AddPoint;
        Map.PlayfieldScaleEventUpdated += UpdatePoint;
        Map.PlayfieldScaleEventRemoved += RemovePoint;
        Map.MapEvents.PlayfieldScaleEvents.ForEach(AddPoint);

        Map.HitObjectEaseEventAdded += AddPoint;
        Map.HitObjectEaseEventUpdated += UpdatePoint;
        Map.HitObjectEaseEventRemoved += RemovePoint;
        Map.MapEvents.HitObjectEaseEvents.ForEach(AddPoint);

        Map.LayerFadeEventAdded += AddPoint;
        Map.LayerFadeEventUpdated += UpdatePoint;
        Map.LayerFadeEventRemoved += RemovePoint;
        Map.MapEvents.LayerFadeEvents.ForEach(AddPoint);

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

        Map.ScrollMultiplierEventAdded += AddPoint;
        Map.ScrollMultiplierEventUpdated += UpdatePoint;
        Map.ScrollMultiplierEventRemoved += RemovePoint;
        Map.MapEvents.ScrollMultiplyEvents.ForEach(AddPoint);

        Map.TimeOffsetEventAdded += AddPoint;
        Map.TimeOffsetEventUpdated += UpdatePoint;
        Map.TimeOffsetEventRemoved += RemovePoint;
        Map.MapEvents.TimeOffsetEvents.ForEach(AddPoint);

        Map.ScriptEventAdded += AddPoint;
        Map.ScriptEventUpdated += UpdatePoint;
        Map.ScriptEventRemoved += RemovePoint;
        Map.MapEvents.ScriptEvents.ForEach(AddPoint);

        Map.NoteEventAdded += AddPoint;
        Map.NoteEventUpdated += UpdatePoint;
        Map.NoteEventRemoved += RemovePoint;
        Map.MapEvents.NoteEvents.ForEach(AddPoint);
    }

    protected override PointListEntry CreateEntryFor(ITimedObject obj) => obj switch
    {
        ScrollVelocity scroll => new ScrollVelocityEntry(scroll),
        FlashEvent flash => new FlashEntry(flash),
        PulseEvent pulse => new PulseEntry(pulse),
        ShakeEvent shake => new ShakeEntry(shake),
        PlayfieldMoveEvent move => new PlayfieldMoveEntry(move),
        PlayfieldScaleEvent scale => new PlayfieldScaleEntry(scale),
        LayerFadeEvent fade => new LayerFadeEntry(fade),
        HitObjectEaseEvent ease => new HitObjectEaseEntry(ease),
        BeatPulseEvent pulse => new BeatPulseEntry(pulse),
        PlayfieldRotateEvent rotate => new PlayfieldRotateEntry(rotate),
        ShaderEvent shader => new ShaderEntry(shader),
        ScrollMultiplierEvent scroll => new ScrollMultiplierEntry(scroll),
        TimeOffsetEvent offset => new TimeOffsetEntry(offset),
        ScriptEvent script => new ScriptEntry(script),
        NoteEvent note => new NoteEntry(note),
        _ => null
    };

    protected override IEnumerable<DropdownEntry> CreateDropdownEntries()
    {
        var entries = new List<DropdownEntry>
        {
            new("Scroll Velocity", FluXisColors.ScrollVelocity, () => Create(new ScrollVelocity()), x => x is ScrollVelocity),
            new("Flash", FluXisColors.Flash, () => Create(new FlashEvent()), x => x is FlashEvent),
            new("Pulse", FluXisColors.Pulse, () => Create(new PulseEvent()), x => x is PulseEvent),
            new("Shake", FluXisColors.Shake, () => Create(new ShakeEvent()), x => x is ShakeEvent),
            new("Playfield Move", FluXisColors.PlayfieldMove, () => Create(new PlayfieldMoveEvent()), x => x is PlayfieldMoveEvent),
            new("Playfield Scale", FluXisColors.PlayfieldScale, () => Create(new PlayfieldScaleEvent()), x => x is PlayfieldScaleEvent),
            new("Playfield Rotate", FluXisColors.PlayfieldRotate, () => Create(new PlayfieldRotateEvent()), x => x is PlayfieldRotateEvent),
            new("HitObject Ease", FluXisColors.HitObjectEase, () => Create(new HitObjectEaseEvent()), x => x is HitObjectEaseEvent),
            new("Layer Fade", FluXisColors.LayerFade, () => Create(new LayerFadeEvent()), x => x is LayerFadeEvent),
            new("Beat Pulse", FluXisColors.BeatPulse, () => Create(new BeatPulseEvent()), x => x is BeatPulseEvent),
            new("Shader", FluXisColors.Shader, () => Create(new ShaderEvent { ShaderName = "Bloom" }), x => x is ShaderEvent),
            new("Scroll Multiplier", FluXisColors.ScrollMultiply, () => Create(new ScrollMultiplierEvent()), x => x is ScrollMultiplierEvent),
            new("Time Offset", FluXisColors.TimeOffset, () => Create(new TimeOffsetEvent()), x => x is TimeOffsetEvent),
            // new("Script", FluXisColors.Script, () => Create(new ScriptEvent()), x => x is ScriptEvent),
            new("Note", FluXisColors.Note, () => Create(new NoteEvent()), x => x is NoteEvent),
        };

        return entries;
    }
}

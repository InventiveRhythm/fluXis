using System.Collections.Generic;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Structures;
using fluXis.Map.Structures.Bases;
using fluXis.Map.Structures.Events;
using fluXis.Map.Structures.Events.Playfields;
using fluXis.Map.Structures.Events.Scrolling;
using fluXis.Screens.Edit.Tabs.Design.Points.Entries;
using fluXis.Screens.Edit.Tabs.Design.Points.Entries.Playfields;
using fluXis.Screens.Edit.Tabs.Design.Points.Entries.Scrolling;
using fluXis.Screens.Edit.Tabs.Shared.Points.List;

namespace fluXis.Screens.Edit.Tabs.Design.Points;

public partial class DesignPointsList : PointsList
{
    protected override void RegisterEvents()
    {
        RegisterTypeEvents(Map.MapInfo.ScrollVelocities);
        RegisterTypeEvents(Map.MapEvents.FlashEvents);
        RegisterTypeEvents(Map.MapEvents.ColorFadeEvents);
        RegisterTypeEvents(Map.MapEvents.PulseEvents);
        RegisterTypeEvents(Map.MapEvents.ShakeEvents);
        RegisterTypeEvents(Map.MapEvents.PlayfieldMoveEvents);
        RegisterTypeEvents(Map.MapEvents.PlayfieldScaleEvents);
        RegisterTypeEvents(Map.MapEvents.HitObjectEaseEvents);
        RegisterTypeEvents(Map.MapEvents.LayerFadeEvents);
        RegisterTypeEvents(Map.MapEvents.ShaderEvents);
        RegisterTypeEvents(Map.MapEvents.BeatPulseEvents);
        RegisterTypeEvents(Map.MapEvents.PlayfieldRotateEvents);
        RegisterTypeEvents(Map.MapEvents.ScrollMultiplyEvents);
        RegisterTypeEvents(Map.MapEvents.TimeOffsetEvents);
        RegisterTypeEvents(Map.MapEvents.ScriptEvents);
        RegisterTypeEvents(Map.MapEvents.NoteEvents);
    }

    protected override PointListEntry CreateEntryFor(ITimedObject obj) => obj switch
    {
        ScrollVelocity scroll => new ScrollVelocityEntry(scroll),
        FlashEvent flash => new FlashEntry(flash),
        ColorFadeEvent colorFade => new ColorFadeEntry(colorFade),
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
            new("Scroll Velocity", Theme.ScrollVelocity, () => Create(new ScrollVelocity()), x => x is ScrollVelocity),
            new("Flash", Theme.Flash, () => Create(new FlashEvent()), x => x is FlashEvent),
            new("Color Fade", Theme.ColorFade, () => Create(new ColorFadeEvent()), x => x is ColorFadeEvent),
            new("Pulse", Theme.Pulse, () => Create(new PulseEvent()), x => x is PulseEvent),
            new("Shake", Theme.Shake, () => Create(new ShakeEvent()), x => x is ShakeEvent),
            new("Playfield Move", Theme.PlayfieldMove, () => Create(new PlayfieldMoveEvent()), x => x is PlayfieldMoveEvent),
            new("Playfield Scale", Theme.PlayfieldScale, () => Create(new PlayfieldScaleEvent()), x => x is PlayfieldScaleEvent),
            new("Playfield Rotate", Theme.PlayfieldRotate, () => Create(new PlayfieldRotateEvent()), x => x is PlayfieldRotateEvent),
            new("HitObject Ease", Theme.HitObjectEase, () => Create(new HitObjectEaseEvent()), x => x is HitObjectEaseEvent),
            new("Layer Fade", Theme.LayerFade, () => Create(new LayerFadeEvent()), x => x is LayerFadeEvent),
            new("Beat Pulse", Theme.BeatPulse, () => Create(new BeatPulseEvent()), x => x is BeatPulseEvent),
            new("Shader", Theme.Shader, () => Create(new ShaderEvent { ShaderName = "Bloom" }), x => x is ShaderEvent),
            new("Scroll Multiplier", Theme.ScrollMultiply, () => Create(new ScrollMultiplierEvent()), x => x is ScrollMultiplierEvent),
            new("Time Offset", Theme.TimeOffset, () => Create(new TimeOffsetEvent()), x => x is TimeOffsetEvent),
            // new("Script", FluXisColors.Script, () => Create(new ScriptEvent()), x => x is ScriptEvent),
            new("Note", Theme.Note, () => Create(new NoteEvent()), x => x is NoteEvent),
        };

        return entries;
    }
}

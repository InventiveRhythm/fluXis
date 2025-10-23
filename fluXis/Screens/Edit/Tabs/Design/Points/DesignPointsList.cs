using System.Collections.Generic;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Structures;
using fluXis.Map.Structures.Bases;
using fluXis.Map.Structures.Events;
using fluXis.Map.Structures.Events.Camera;
using fluXis.Map.Structures.Events.Playfields;
using fluXis.Map.Structures.Events.Scrolling;
using fluXis.Screens.Edit.Tabs.Design.Points.Entries;
using fluXis.Screens.Edit.Tabs.Design.Points.Entries.Camera;
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
        RegisterTypeEvents(Map.MapEvents.CameraMoveEvents);
        RegisterTypeEvents(Map.MapEvents.CameraScaleEvents);
        RegisterTypeEvents(Map.MapEvents.CameraRotateEvents);
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
        CameraMoveEvent camm => new CameraMoveEntry(camm),
        CameraScaleEvent cams => new CameraScaleEntry(cams),
        CameraRotateEvent camr => new CameraRotateEntry(camr),
        ScriptEvent script => new ScriptEntry(script),
        NoteEvent note => new NoteEntry(note),
        _ => null
    };

    protected override IEnumerable<DropdownEntry> CreateDropdownEntries() => new List<DropdownEntry>
    {
        CreateDefaultDropdownEntry<ScrollVelocity>("Scroll Velocity", Theme.ScrollVelocity),
        CreateDefaultDropdownEntry<FlashEvent>("Flash", Theme.Flash),
        CreateDefaultDropdownEntry<ColorFadeEvent>("Color Fade", Theme.ColorFade),
        CreateDefaultDropdownEntry<PulseEvent>("Pulse", Theme.Pulse),
        CreateDefaultDropdownEntry<ShakeEvent>("Shake", Theme.Shake),
        CreateDefaultDropdownEntry<PlayfieldMoveEvent>("Playfield Move", Theme.PlayfieldMove),
        CreateDefaultDropdownEntry<PlayfieldScaleEvent>("Playfield Scale", Theme.PlayfieldScale),
        CreateDefaultDropdownEntry<PlayfieldRotateEvent>("Playfield Rotate", Theme.PlayfieldRotate),
        CreateDefaultDropdownEntry<HitObjectEaseEvent>("HitObject Ease", Theme.HitObjectEase),
        CreateDefaultDropdownEntry<LayerFadeEvent>("Layer Fade", Theme.LayerFade),
        CreateDefaultDropdownEntry<BeatPulseEvent>("Beat Pulse", Theme.BeatPulse),
        CreateDefaultDropdownEntry<ShaderEvent>("Shader", Theme.Shader),
        CreateDefaultDropdownEntry<ScrollMultiplierEvent>("Scroll Multiplier", Theme.ScrollMultiply),
        CreateDefaultDropdownEntry<TimeOffsetEvent>("Time Offset", Theme.TimeOffset),
        CreateDefaultDropdownEntry<CameraMoveEvent>("Camera Move", Theme.CameraMove),
        CreateDefaultDropdownEntry<CameraScaleEvent>("Camera Scale", Theme.CameraScale),
        CreateDefaultDropdownEntry<CameraRotateEvent>("Camera Rotate", Theme.CameraRotate),
        // CreateDefaultDropdownEntry<ScriptEvent>("Script", FluXisColors.Script),
        CreateDefaultDropdownEntry<NoteEvent>("Note", Theme.Note),
    };
}

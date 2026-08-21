using System.Collections.Generic;
using System.Reflection;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Structures;
using fluXis.Map.Structures.Bases;
using fluXis.Map.Structures.Events;
using fluXis.Map.Structures.Events.Camera;
using fluXis.Map.Structures.Events.Groups;
using fluXis.Map.Structures.Events.Playfields;
using fluXis.Map.Structures.Events.Scrolling;
using fluXis.Screens.Edit.Tabs.Charting.Points.Entries;
using fluXis.Screens.Edit.Tabs.Charting.Points.Entries.Event;
using fluXis.Screens.Edit.Tabs.Charting.Points.Entries.Event.Camera;
using fluXis.Screens.Edit.Tabs.Charting.Points.Entries.Event.Groups;
using fluXis.Screens.Edit.Tabs.Charting.Points.Entries.Event.Playfields;
using fluXis.Screens.Edit.Tabs.Charting.Points.Entries.Event.Scrolling;
using fluXis.Screens.Edit.Tabs.Shared.Points.List;

namespace fluXis.Screens.Edit.Tabs.Charting.Points;

public partial class ChartingPointsList : PointsList
{
    protected override void RegisterEvents()
    {
        RegisterTypeEvents(Map.MapInfo.TimingPoints);
        RegisterTypeEvents(Map.MapInfo.ScrollVelocities);

        foreach (var (type, list) in Map.MapEvents.GetListsForTypes())
        {
            var method = GetType().GetMethod(nameof(registerEffect), BindingFlags.Instance | BindingFlags.NonPublic)!;
            method = method.MakeGenericMethod(type);
            method.Invoke(this, [list]);
        }
    }

    private void registerEffect<T>(List<T> list)
        where T : class, ITimedObject
        => RegisterTypeEvents(list);

    protected override PointListEntry CreateEntryFor(ITimedObject obj) => obj switch
    {
        TimingPoint timing => new TimingPointEntry(timing),
        LaneSwitchEvent lane => new LaneSwitchEntry(lane),

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
        LoopEvent loop => new LoopEventEntry(loop),
        NoteEvent note => new NoteEntry(note),
        _ => null
    };

    protected override IEnumerable<DropdownEntry> CreateDropdownEntries() => new[]
    {
        CreateDefaultDropdownEntry<TimingPoint>("Timing Point", Theme.TimingPoint),
        CreateDefaultDropdownEntry<LaneSwitchEvent>("Lane Switch", Theme.LaneSwitch, l => l.Count = Map.RealmMap.KeyCount),

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
        CreateDefaultDropdownEntry<LoopEvent>("Loop", Theme.Loop),
        CreateDefaultDropdownEntry<NoteEvent>("Note", Theme.Note),
    };
}

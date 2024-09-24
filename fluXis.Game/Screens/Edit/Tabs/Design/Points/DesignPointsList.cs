using System.Collections.Generic;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Map.Structures.Bases;
using fluXis.Game.Map.Structures.Events;
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

        Map.HitObjectFadeEventAdded += AddPoint;
        Map.HitObjectFadeEventUpdated += UpdatePoint;
        Map.HitObjectFadeEventRemoved += RemovePoint;
        Map.MapEvents.HitObjectFadeEvents.ForEach(AddPoint);

        Map.HitObjectEaseEventAdded += AddPoint;
        Map.HitObjectEaseEventUpdated += UpdatePoint;
        Map.HitObjectEaseEventRemoved += RemovePoint;
        Map.MapEvents.HitObjectEaseEvents.ForEach(AddPoint);

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
            HitObjectFadeEvent fade => new HitObjectFadeEntry(fade),
            HitObjectEaseEvent ease => new HitObjectEaseEntry(ease),
            BeatPulseEvent pulse => new BeatPulseEntry(pulse),
            PlayfieldRotateEvent rotate => new PlayfieldRotateEntry(rotate),
            ShaderEvent shader => new ShaderEntry(shader),
            ScrollMultiplierEvent scroll => new ScrollMultiplierEntry(scroll),
            NoteEvent note => new NoteEntry(note),
            _ => null
        };
    }

    protected override IEnumerable<AddButtonEntry> CreateAddEntries()
    {
        var entries = new List<AddButtonEntry>
        {
            new("Flash", FluXisColors.Flash, () => Create(new FlashEvent())),
            new("Shake", FluXisColors.Shake, () => Create(new ShakeEvent())),
            new("Playfield Move", FluXisColors.PlayfieldMove, () => Create(new PlayfieldMoveEvent())),
            new("Playfield Scale", FluXisColors.PlayfieldScale, () => Create(new PlayfieldScaleEvent())),
            new("Playfield Rotate", FluXisColors.PlayfieldRotate, () => Create(new PlayfieldRotateEvent())),
            new("Playfield Fade", FluXisColors.PlayfieldFade, () => Create(new PlayfieldFadeEvent())),
            new("HitObject Fade", FluXisColors.HitObjectFade, () => Create(new HitObjectFadeEvent())),
            new("HitObject Ease", FluXisColors.HitObjectEase, () => Create(new HitObjectEaseEvent())),
            new("Beat Pulse", FluXisColors.BeatPulse, () => Create(new BeatPulseEvent())),
            new("Shader", FluXisColors.Shader, () => Create(new ShaderEvent { ShaderName = "Bloom" })),
            new("Scroll Multiplier", FluXisColors.PlayfieldRotate, () => Create(new ScrollMultiplierEvent())),
            new("Note", FluXisColors.Note, () => Create(new NoteEvent())),
        };

        return entries;
    }
}

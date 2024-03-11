using System;
using fluXis.Game.Map;
using fluXis.Game.Map.Events;
using fluXis.Game.Map.Structures;

namespace fluXis.Game.Screens.Edit;

public class EditorMapEvents : MapEvents
{
    public event Action<LaneSwitchEvent> LaneSwitchEventAdded;
    public event Action<LaneSwitchEvent> LaneSwitchEventChanged;
    public event Action<LaneSwitchEvent> LaneSwitchEventRemoved;

    public event Action<FlashEvent> FlashEventAdded;
    public event Action<FlashEvent> FlashEventRemoved;
    public event Action<PulseEvent> PulseEventAdded;
    public event Action<PulseEvent> PulseEventRemoved;
    public event Action<PlayfieldMoveEvent> PlayfieldMoveEventAdded;
    public event Action<PlayfieldMoveEvent> PlayfieldMoveEventRemoved;
    public event Action<ShakeEvent> ShakeEventAdded;
    public event Action<ShakeEvent> ShakeEventRemoved;

    public void Update(ITimedObject obj)
    {
        switch (obj)
        {
            case LaneSwitchEvent lane:
                LaneSwitchEventChanged?.Invoke(lane);
                break;
        }
    }

    public void Add(LaneSwitchEvent laneSwitchEvent)
    {
        LaneSwitchEvents.Add(laneSwitchEvent);
        LaneSwitchEventAdded?.Invoke(laneSwitchEvent);
    }

    public void Remove(LaneSwitchEvent laneSwitchEvent)
    {
        LaneSwitchEvents.Remove(laneSwitchEvent);
        LaneSwitchEventRemoved?.Invoke(laneSwitchEvent);
    }

    public void Add(FlashEvent flashEvent)
    {
        FlashEvents.Add(flashEvent);
        FlashEventAdded?.Invoke(flashEvent);
    }

    public void Remove(FlashEvent flashEvent)
    {
        FlashEvents.Remove(flashEvent);
        FlashEventRemoved?.Invoke(flashEvent);
    }

    public void Add(PulseEvent pulseEvent)
    {
        PulseEvents.Add(pulseEvent);
        PulseEventAdded?.Invoke(pulseEvent);
    }

    public void Remove(PulseEvent pulseEvent)
    {
        PulseEvents.Remove(pulseEvent);
        PulseEventRemoved?.Invoke(pulseEvent);
    }

    public void Add(PlayfieldMoveEvent playfieldMoveEvent)
    {
        PlayfieldMoveEvents.Add(playfieldMoveEvent);
        PlayfieldMoveEventAdded?.Invoke(playfieldMoveEvent);
    }

    public void Remove(PlayfieldMoveEvent playfieldMoveEvent)
    {
        PlayfieldMoveEvents.Remove(playfieldMoveEvent);
        PlayfieldMoveEventRemoved?.Invoke(playfieldMoveEvent);
    }

    public void Add(ShakeEvent shakeEvent)
    {
        ShakeEvents.Add(shakeEvent);
        ShakeEventAdded?.Invoke(shakeEvent);
    }

    public void Remove(ShakeEvent shakeEvent)
    {
        ShakeEvents.Remove(shakeEvent);
        ShakeEventRemoved?.Invoke(shakeEvent);
    }

    public void ApplyOffsetToAll(float offset)
    {
        foreach (var switchEvent in LaneSwitchEvents)
        {
            switchEvent.Time += offset;
            Update(switchEvent);
        }

        foreach (var flashEvent in FlashEvents)
        {
            flashEvent.Time += offset;
            Update(flashEvent);
        }

        foreach (var pulseEvent in PulseEvents)
        {
            pulseEvent.Time += offset;
            Update(pulseEvent);
        }

        foreach (var moveEvent in PlayfieldMoveEvents)
        {
            moveEvent.Time += offset;
            Update(moveEvent);
        }

        foreach (var scaleEvent in PlayfieldScaleEvents)
        {
            scaleEvent.Time += offset;
            Update(scaleEvent);
        }

        foreach (var shakeEvent in ShakeEvents)
        {
            shakeEvent.Time += offset;
            Update(shakeEvent);
        }

        foreach (var fadeEvent in PlayfieldFadeEvents)
        {
            fadeEvent.Time += offset;
            Update(fadeEvent);
        }

        foreach (var shaderEvent in ShaderEvents)
        {
            shaderEvent.Time += offset;
            Update(shaderEvent);
        }
    }
}

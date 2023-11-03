using System;
using System.Collections.Generic;
using fluXis.Game.Map;
using fluXis.Game.Map.Events;

namespace fluXis.Game.Screens.Edit;

public class EditorMapEvents : MapEvents
{
    public event Action<LaneSwitchEvent> LaneSwitchEventAdded;
    public event Action<LaneSwitchEvent> LaneSwitchEventRemoved;
    public event Action<FlashEvent> FlashEventAdded;
    public event Action<FlashEvent> FlashEventRemoved;
    public event Action<PulseEvent> PulseEventAdded;
    public event Action<PulseEvent> PulseEventRemoved;
    public event Action<PlayfieldMoveEvent> PlayfieldMoveEventAdded;
    public event Action<PlayfieldMoveEvent> PlayfieldMoveEventRemoved;
    public event Action<ShakeEvent> ShakeEventAdded;
    public event Action<ShakeEvent> ShakeEventRemoved;

    public EditorMapEvents()
    {
        LaneSwitchEvents = new List<LaneSwitchEvent>();
        FlashEvents = new List<FlashEvent>();
        PulseEvents = new List<PulseEvent>();
        PlayfieldMoveEvents = new List<PlayfieldMoveEvent>();
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
}

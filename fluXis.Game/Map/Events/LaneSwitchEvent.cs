namespace fluXis.Game.Map.Events;

public class LaneSwitchEvent : TimedObject
{
    public int Count { get; set; }
    public float Speed { get; set; } = 250;
}

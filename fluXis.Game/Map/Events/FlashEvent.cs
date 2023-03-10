namespace fluXis.Game.Map.Events;

public class FlashEvent : TimedObject
{
    public float FadeInTime { get; set; }
    public float HoldTime { get; set; }
    public float FadeOutTime { get; set; }
}

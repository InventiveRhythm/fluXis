namespace fluXis.Game.Map
{
    public class HitObjectInfo
    {
        public int Time;
        public int Lane;
        public int HoldTime;

        public int HoldEndTime => Time + HoldTime;

        public bool IsLongNote()
        {
            return HoldTime > 0;
        }

        public override string ToString()
        {
            return $"Time: {Time}, Lane: {Lane}, HoldTime: {HoldTime}";
        }
    }
}

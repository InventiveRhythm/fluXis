namespace fluXis.Game.Map
{
    public class ScrollVelocityInfo
    {
        public int Time { get; set; }
        public float Multiplier { get; set; }

        public ScrollVelocityInfo(int time, float multiplier)
        {
            Time = time;
            Multiplier = multiplier;
        }
    }
}

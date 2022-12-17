namespace fluXis.Game.Map
{
    public class ScrollVelocityInfo
    {
        public float Time { get; set; }
        public float Multiplier { get; set; }

        public ScrollVelocityInfo(float time, float multiplier)
        {
            Time = time;
            Multiplier = multiplier;
        }
    }
}

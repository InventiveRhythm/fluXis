namespace fluXis.Game.Map
{
    public class TimingPointInfo
    {
        public int Time;
        public int BPM;
        public TimeSignature Signature;
    }

    public enum TimeSignature
    {
        Quad = 4,
        Triple = 3
    }
}

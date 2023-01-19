namespace fluXis.Game.Map
{
    public class TimingPointInfo
    {
        public float Time;
        public int BPM;
        public int Signature;

        public float GetMsPerBeat()
        {
            return 60000f / BPM;
        }
    }
}


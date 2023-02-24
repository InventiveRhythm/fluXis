namespace fluXis.Game.Map
{
    public class TimingPointInfo
    {
        public float Time;
        public float BPM;
        public int Signature;
        public bool HideLines;

        public float GetMsPerBeat()
        {
            return 60000f / BPM;
        }
    }
}


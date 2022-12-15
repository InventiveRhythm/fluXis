namespace fluXis.Game.Map
{
    public class TimingPointInfo
    {
        public int Time;
        public int BPM;
        public int Signature;

        public int GetMsPerBeat()
        {
            return 60000 / BPM;
        }
    }
}


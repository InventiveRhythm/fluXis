using System;
using System.Text.Json.Serialization;

namespace fluXis.Game.Map
{
    public class TimingPointInfo
    {
        public float Time;
        public float BPM;
        public int Signature;
        public bool HideLines;

        [JsonIgnore]
        public float MsPerBeat => 60000f / BPM;

        [Obsolete("Use MsPerBeat instead")]
        public float GetMsPerBeat()
        {
            return 60000f / BPM;
        }
    }
}


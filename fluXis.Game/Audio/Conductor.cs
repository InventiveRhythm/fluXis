using osu.Framework.Audio.Track;
using osu.Framework.Timing;

namespace fluXis.Game.Audio
{
    public class Conductor
    {
        private static Track track;
        public static int Time;

        public static void Update(FrameTimeInfo time)
        {
            if (Time < 0)
            {
                Time += (int)time.Elapsed;
                return;
            }

            if (track != null)
            {
                Time = (int)track.CurrentTime;
            }
            else
            {
                Time += (int)time.Elapsed;
            }
        }

        public static void PlayTrack(ITrackStore store, string path, bool start = false, int time = 0)
        {
            if (track != null)
            {
                track.Stop();
                track.Dispose();
            }

            track = store.Get(path);

            if (start)
                track.Start();
            else
                track.Seek(time);
        }

        public static void PauseTrack()
        {
            track?.Stop();
        }

        public static void ResumeTrack()
        {
            track?.Start();
        }
    }
}

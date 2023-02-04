using System;
using fluXis.Game.Database.Maps;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Transforms;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;

namespace fluXis.Game.Audio
{
    public partial class Conductor : Container
    {
        private static ITrackStore trackStore;
        private static Track track;
        public static float CurrentTime;
        public static int Offset = 0;

        public static LowPassFilter LowPassFilter;

        public static float Speed
        {
            get => instance.untweenedSpeed;
            set => SetSpeed(value);
        }

        public static bool IsPlaying => track?.IsRunning ?? false;

        private const float max_speed = 2.0f;
        private const float min_speed = 0.5f;

        private static readonly BindableNumber<double> bind_speed = new BindableDouble(1);
        private static Conductor instance;
        private float speed = 1;
        private float untweenedSpeed = 1;

        public static Action<int> OnBeat;
        public static float StepTime => instance.stepTime;
        public static float BeatTime => instance.stepTime * 4;

        [BackgroundDependencyLoader]
        private void load(AudioManager audioManager, Storage storage)
        {
            instance = this;
            trackStore = audioManager.GetTrackStore(new StorageBackedResourceStore(storage.GetStorageForDirectory("files")));
            Add(LowPassFilter = new LowPassFilter(audioManager.TrackMixer));
        }

        protected override void Update()
        {
            bind_speed.Value = speed;

            if (CurrentTime < 0)
            {
                CurrentTime += (float)Time.Elapsed;
                return;
            }

            if (track != null)
            {
                CurrentTime = (float)track.CurrentTime + Offset;
            }
            else
            {
                CurrentTime += (float)Time.Elapsed + Offset;
            }

            updateStep();
        }

        public static void PlayTrack(RealmMap info, bool start = false, int time = 0)
        {
            if (track != null)
            {
                track.Stop();
                track.Dispose();
            }

            track = trackStore.Get(info.MapSet.GetFile(info.Metadata.Audio)?.GetPath()) ?? trackStore.GetVirtual();
            track.AddAdjustment(AdjustableProperty.Frequency, bind_speed);

            track.Seek(time);

            if (start)
                track.Start();
        }

        public static void Seek(float time)
        {
            track?.Seek(time);
        }

        public static void PauseTrack()
        {
            track?.Stop();
        }

        public static void ResumeTrack()
        {
            track?.Start();
        }

        public static void SetLoop(int start)
        {
            if (track == null)
                return;

            track.Looping = true;
            track.RestartPoint = start;
        }

        public static void ResetLoop()
        {
            if (track == null)
                return;

            track.Looping = false;
        }

        public static TransformSequence<Conductor> SetSpeed(float newSpeed, int duration = 400, Easing ease = Easing.OutQuint, bool force = false)
        {
            // make an exception when pausing
            if (newSpeed != 0 || force)
            {
                if (newSpeed > max_speed)
                    newSpeed = max_speed;
                else if (newSpeed < min_speed)
                    newSpeed = min_speed;
            }

            instance.untweenedSpeed = newSpeed;
            return instance.TransformTo(nameof(speed), newSpeed, duration, ease);
        }

        public static void AddSpeed(float addSpeed, int duration = 400, Easing ease = Easing.OutQuint)
        {
            SetSpeed(instance.untweenedSpeed + addSpeed, duration, ease);
        }

        private int lastStep;
        private int step;
        private float stepTime;

        // gonna be used somewhere later
        private void updateStep()
        {
            // TODO: rewrite this
            lastStep = step;
            step = 0;
            stepTime = 0;

            /*if (map == null)
                return;

            stepTime = 60000f / map.GetTimingPoint(CurrentTime).BPM / 4;
            lastStep = step;
            step = (int)(CurrentTime / stepTime);

            if (lastStep != step && step % 4 == 0)
                OnBeat?.Invoke(step / 4);*/
        }
    }
}

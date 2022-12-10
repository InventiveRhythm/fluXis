using System.IO;
using fluXis.Game.Map;
using fluXis.Game.Screens.Gameplay.Ruleset;
using fluXis.Game.Audio;
using fluXis.Game.Scoring;
using fluXis.Game.Screens.Gameplay.HUD;
using fluXis.Game.Screens.Gameplay.Input;
using fluXis.Game.Screens.Gameplay.UI;
using fluXis.Game.Screens.Result;
using Newtonsoft.Json;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;
using osu.Framework.Screens;
using osuTK.Input;

namespace fluXis.Game.Screens.Gameplay
{
    public class GameplayScreen : Screen
    {
        private Playfield playfield { get; set; }
        private bool starting = true;
        private bool ended;

        public GameplayInput Input;
        public Performance Performance;
        public MapInfo Map;
        public JudgementDisplay JudgementDisplay;

        public Sample HitSound;
        public Sample Combobreak;
        public Sample Restart;

        [BackgroundDependencyLoader]
        private void load(ITrackStore tracks, ISampleStore samples)
        {
            Input = new GameplayInput();
            Performance = new Performance();

            tracks.Volume.Value = 0.1f;
            samples.Volume.Value = 0.1f;

            HitSound = samples.Get("gameplay/hit.ogg");
            Combobreak = samples.Get("gameplay/combobreak.ogg");
            Restart = samples.Get("gameplay/restart.ogg");

            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;

            InternalChildren = new Drawable[]
            {
                playfield = new Playfield(this),
                new PauseMenu(this)
            };

            AddInternal(new ComboCounter(this));
            AddInternal(new AccuracyDisplay(this));
            AddInternal(new Progressbar(this));
            AddInternal(JudgementDisplay = new JudgementDisplay(this));

            Map = JsonConvert.DeserializeObject<MapInfo>(File.ReadAllText("C:/Users/Flux/AppData/Roaming/fluXis/maps/polkamania/hyper.fsc"));

            playfield.LoadMap(Map);
            Performance.SetMapInfo(Map);

            Conductor.PlayTrack(tracks, "audio.ogg");
            Conductor.Time = -3000;
        }

        protected override void Update()
        {
            if (Conductor.Time >= 0 && starting)
            {
                starting = false;
                Conductor.ResumeTrack();
            }

            if (!starting && playfield.Manager.IsFinished)
            {
                End();
            }

            Input.Update();

            base.Update();
        }

        public void End()
        {
            if (ended)
                return;

            ended = true;

            this.Push(new ResultsScreen(Map, Performance));
        }

        public override void OnSuspending(ScreenTransitionEvent e)
        {
            fadeOut();
        }

        private void fadeOut()
        {
            playfield.OnExit();
            this.FadeOut(800);
        }

        protected override bool OnKeyDown(KeyDownEvent e)
        {
            Input.OnKeyDown(e);

            if (e.Key == Key.Escape)
                End();

            if (e.Key == Key.ShiftLeft)
            {
                Restart.Play();
                Conductor.Time = 0;
                this.Push(new GameplayScreen());
            }

            return base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyUpEvent e)
        {
            Input.OnKeyUp(e);
        }
    }
}

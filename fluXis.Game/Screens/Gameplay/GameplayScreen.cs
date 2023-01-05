using fluXis.Game.Map;
using fluXis.Game.Screens.Gameplay.Ruleset;
using fluXis.Game.Audio;
using fluXis.Game.Integration;
using fluXis.Game.Scoring;
using fluXis.Game.Screens.Gameplay.HUD;
using fluXis.Game.Screens.Gameplay.HUD.Judgement;
using fluXis.Game.Screens.Gameplay.Input;
using fluXis.Game.Screens.Gameplay.UI;
using fluXis.Game.Screens.Result;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;
using osu.Framework.Screens;
using osuTK.Input;

namespace fluXis.Game.Screens.Gameplay
{
    public class GameplayScreen : Screen
    {
        private bool starting = true;
        private bool ended;
        private bool restarting;
        private bool paused;

        public GameplayInput Input;
        public Performance Performance;
        public MapInfo Map;
        public JudgementDisplay JudgementDisplay;
        public Playfield Playfield { get; private set; }

        public Sample HitSound;
        public Sample Combobreak;
        public Sample Restart;

        public GameplayScreen(MapInfo map)
        {
            Map = map;
        }

        [BackgroundDependencyLoader]
        private void load(ISampleStore samples)
        {
            Input = new GameplayInput();
            Performance = new Performance();

            HitSound = samples.Get("gameplay/hit.ogg");
            Combobreak = samples.Get("gameplay/combobreak.ogg");
            Restart = samples.Get("gameplay/restart.ogg");

            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;

            InternalChildren = new Drawable[]
            {
                Playfield = new Playfield(this),
                new PauseMenu(this)
            };

            AddInternal(new ComboCounter(this));
            AddInternal(new AccuracyDisplay(this));
            AddInternal(new Progressbar(this));
            AddInternal(JudgementDisplay = new JudgementDisplay(this));
            AddInternal(new AutoPlayDisplay(this));
            AddInternal(new JudgementCounter(Performance));

            Playfield.LoadMap(Map);
            Performance.SetMapInfo(Map);
            Conductor.CurrentTime = -100000; // should be enough to load the map right?
            Conductor.PlayTrack(Map);

            Discord.Update("Playing a map", $"{Map.Metadata.Title} - {Map.Metadata.Artist} [{Map.Metadata.Difficulty}]", "playing", 0, (int)((Map.EndTime - Conductor.CurrentTime) / 1000));
        }

        protected override void LoadComplete()
        {
            Conductor.CurrentTime = -2000;

            base.LoadComplete();
        }

        protected override void Update()
        {
            if (Conductor.CurrentTime >= 0 && starting)
            {
                starting = false;
                Conductor.ResumeTrack();
            }

            if (!starting && Playfield.Manager.IsFinished)
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

        public override void OnEntering(ScreenTransitionEvent e)
        {
            Alpha = 0;
            this.ScaleTo(1.2f)
                .ScaleTo(1f, 750, Easing.OutQuint)
                .Delay(250)
                .FadeIn(250);

            base.OnEntering(e);
        }

        private void fadeOut()
        {
            this.FadeOut(restarting ? 0 : 250);
        }

        protected override bool OnKeyDown(KeyDownEvent e)
        {
            Input.OnKeyDown(e);

            if (e.Key == Key.Escape)
                End();

            if (e.Key == Key.Space)
            {
                Conductor.SetSpeed(paused ? 1 : 0);
                paused = !paused;
            }

            if (e.Key == Key.ShiftLeft)
            {
                restarting = true;
                Restart.Play();
                this.Push(new GameplayScreen(Map));
            }

            if (e.Key == Key.F1)
            {
                Playfield.Manager.AutoPlay.Value = !Playfield.Manager.AutoPlay.Value;
            }

            switch (e.Key)
            {
                case Key.Left:
                    Conductor.AddSpeed(-.1f);
                    break;

                case Key.Right:
                    Conductor.AddSpeed(.1f);
                    break;
            }

            return base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyUpEvent e)
        {
            Input.OnKeyUp(e);
        }

        public override void OnResuming(ScreenTransitionEvent e)
        {
            // probably coming back from results screen
            // so just go to song select
            this.Exit();

            base.OnResuming(e);
        }
    }
}

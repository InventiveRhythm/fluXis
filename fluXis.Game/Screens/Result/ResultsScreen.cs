using fluXis.Game.Input;
using fluXis.Game.Integration;
using fluXis.Game.Map;
using fluXis.Game.Online.Scores;
using fluXis.Game.Scoring;
using fluXis.Game.Screens.Result.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Logging;
using osu.Framework.Screens;

namespace fluXis.Game.Screens.Result
{
    public class ResultsScreen : Screen, IKeyBindingHandler<FluXisKeybind>
    {
        private readonly MapInfo map;
        private readonly Performance performance;

        public ResultsScreen(MapInfo map, Performance performance)
        {
            this.map = map;
            this.performance = performance;

            OnlineScores.UploadScore(performance, res =>
            {
                Logger.Log(res.Message);
            });
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;

            InternalChildren = new Drawable[]
            {
                new Container
                {
                    AutoSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    CornerRadius = 10,
                    Masking = true,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = Colour4.FromHex("#222228"),
                        },
                        new FillFlowContainer()
                        {
                            AutoSizeAxes = Axes.Both,
                            Direction = FillDirection.Vertical,
                            Margin = new MarginPadding(20),
                            Children = new Drawable[]
                            {
                                new ResultTitle(map),
                                new ResultScore(performance),
                                new ResultHitPoints(map, performance)
                            }
                        }
                    }
                }
            };

            Discord.Update("Viewing Results", "", "results");

            // Logger.Log(JsonConvert.SerializeObject(performance, Formatting.None));
        }

        public bool OnPressed(KeyBindingPressEvent<FluXisKeybind> e)
        {
            switch (e.Action)
            {
                case FluXisKeybind.Back:
                    this.Exit();
                    return true;
            }

            return false;
        }

        public void OnReleased(KeyBindingReleaseEvent<FluXisKeybind> e) { }

        public override void OnEntering(ScreenTransitionEvent e)
        {
            this.ScaleTo(0.95f)
                .FadeOut()
                .ScaleTo(1f, 250, Easing.OutQuint)
                .FadeIn(250, Easing.OutQuint);

            base.OnEntering(e);
        }

        public override bool OnExiting(ScreenExitEvent e)
        {
            this.ScaleTo(1.05f, 250, Easing.OutQuint)
                .FadeOut(250, Easing.OutQuint);

            return base.OnExiting(e);
        }
    }
}

using fluXis.Game.Input;
using fluXis.Game.Integration;
using fluXis.Game.Map;
using fluXis.Game.Scoring;
using fluXis.Game.Screens.Result.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Screens;
using osuTK;

namespace fluXis.Game.Screens.Result
{
    public class ResultsScreen : Screen, IKeyBindingHandler<FluXisKeybind>
    {
        private readonly MapInfo map;
        private readonly Performance performance;

        private Box metadataLine;
        private SpriteText songTitle;
        private SpriteText songArtist;
        private SpriteText songDiffMapper;
        private Container judgements;

        public ResultsScreen(MapInfo map, Performance performance)
        {
            this.map = map;
            this.performance = performance;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChildren = new Drawable[]
            {
                new Container
                {
                    Position = new Vector2(20, 20),
                    Children = new Drawable[]
                    {
                        metadataLine = new Box
                        {
                            Width = 5,
                        },
                        songTitle = new SpriteText
                        {
                            X = 15,
                            Text = map.Metadata.Title,
                            Font = new FontUsage("Quicksand", 64f, "SemiBold"),
                        },
                        songArtist = new SpriteText
                        {
                            X = 15,
                            Y = 0,
                            Text = map.Metadata.Artist,
                            Font = new FontUsage("Quicksand", 32f),
                            Alpha = 0
                        },
                        songDiffMapper = new SpriteText
                        {
                            X = 15,
                            Y = 0,
                            Text = $"[{map.Metadata.Difficulty}] mapped by {map.Metadata.Mapper}",
                            Font = new FontUsage("Quicksand", 32f),
                            Alpha = 0
                        }
                    }
                },
                judgements = new Container
                {
                    Anchor = Anchor.BottomLeft,
                    Origin = Anchor.BottomLeft,
                    Position = new Vector2(20, -20)
                }
            };

            for (var i = 0; i < Judgement.LIST.Length; i++)
            {
                Judgement jud = Judgement.LIST[i];
                int count = performance.Judgements.ContainsKey(jud.Key) ? performance.Judgements[jud.Key] : 0;
                judgements.Add(new ResultJudgement(jud, count, i, Judgement.LIST.Length));
            }

            Discord.Update("Viewing Results", "", "results");
        }

        protected override void LoadComplete()
        {
            metadataLine.ResizeTo(new Vector2(5, 0))
                        .ResizeTo(new Vector2(5, 120), 250, Easing.OutQuint);

            songTitle.MoveTo(new Vector2(15, -20))
                     .FadeInFromZero(250)
                     .MoveTo(new Vector2(15, 0), 250, Easing.OutQuint);

            songArtist.MoveTo(new Vector2(15, 56 - 20))
                      .Then(250)
                      .FadeInFromZero(250)
                      .MoveTo(new Vector2(15, 56), 250, Easing.OutQuint);

            songDiffMapper.MoveTo(new Vector2(15, 82 - 20))
                          .Then(500)
                          .FadeInFromZero(250)
                          .MoveTo(new Vector2(15, 82), 250, Easing.OutQuint);

            base.LoadComplete();
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
    }
}

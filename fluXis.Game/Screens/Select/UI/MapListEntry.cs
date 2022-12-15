using fluXis.Game.Graphics.Background;
using fluXis.Game.Map;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Screens.Select.UI
{
    public class MapListEntry : Container
    {
        public readonly SelectScreen Screen;
        private readonly MapSet mapset;
        private readonly int index;

        public bool Selected => Screen?.MapSet == mapset;
        private bool selected = true; // so that the first mapset is selected by default

        private Box dim;
        private Container difficultyContainer;
        private FillFlowContainer<MapDifficultyEntry> difficultyFlow;

        public MapListEntry(SelectScreen screen, MapSet mapset, int index)
        {
            Screen = screen;
            this.mapset = mapset;
            this.index = index;
        }

        [BackgroundDependencyLoader]
        private void load(BackgroundTextureStore backgrounds)
        {
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;

            InternalChildren = new Drawable[]
            {
                difficultyContainer = new Container
                {
                    Masking = true,
                    RelativeSizeAxes = Axes.X,
                    Y = 80,
                    Child = difficultyFlow = new FillFlowContainer<MapDifficultyEntry>
                    {
                        Direction = FillDirection.Vertical,
                        AutoSizeAxes = Axes.Y,
                        RelativeSizeAxes = Axes.X,
                        Spacing = new Vector2(0, 5),
                        Masking = true
                    },
                },
                new Container
                {
                    RelativeSizeAxes = Axes.X,
                    Height = 75,
                    CornerRadius = 10,
                    Margin = new MarginPadding { Bottom = 5 },
                    Masking = true,
                    Children = new Drawable[]
                    {
                        new Sprite
                        {
                            RelativeSizeAxes = Axes.Both,
                            Texture = backgrounds.Get(mapset.GetBackgroundPath()),
                            FillMode = FillMode.Fill,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre
                        },
                        dim = new Box
                        {
                            Name = "Background Dim",
                            RelativeSizeAxes = Axes.Both,
                            Colour = Colour4.Black,
                            Alpha = .4f
                        },
                        new Container
                        {
                            Padding = new MarginPadding(10),
                            RelativeSizeAxes = Axes.Both,
                            Children = new[]
                            {
                                new SpriteText
                                {
                                    Font = new FontUsage("Quicksand", 32f, "SemiBold"),
                                    Text = mapset.Title,
                                    Anchor = Anchor.TopLeft,
                                    Origin = Anchor.TopLeft,
                                    Y = -2
                                },
                                new SpriteText
                                {
                                    Font = new FontUsage("Quicksand", 28f),
                                    Text = mapset.Artist,
                                    Anchor = Anchor.TopLeft,
                                    Origin = Anchor.TopLeft,
                                    Y = 24
                                }
                            }
                        }
                    }
                }
            };

            foreach (var map in mapset.Maps)
            {
                difficultyFlow.Add(new MapDifficultyEntry(this, map));
            }
        }

        protected override void Update()
        {
            if (Selected != selected)
            {
                if (Selected)
                {
                    dim.FadeTo(.2f, 100);
                    difficultyContainer.ResizeHeightTo(difficultyFlow.Height, 200, Easing.OutQuint);
                }
                else
                {
                    dim.FadeTo(.4f, 100);
                    difficultyContainer.ResizeHeightTo(0, 200, Easing.OutQuint);
                }
            }

            selected = Selected;

            base.Update();
        }

        protected override bool OnClick(ClickEvent e)
        {
            if (Selected)
                return false; // dont handle clicks when we already selected this mapset

            Screen.SelectMapSet(mapset);
            return true;
        }

        protected override void LoadComplete()
        {
            const float duration = 200;
            const float delay = 50;

            this.FadeOut()
                .Delay(delay * index)
                .FadeIn(duration);

            base.LoadComplete();
        }

        protected override bool OnHover(HoverEvent e)
        {
            dim.FadeTo(Selected ? .1f : .3f, 100);
            return base.OnHover(e);
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            dim.FadeTo(Selected ? .2f : .4f, 300);
            base.OnHoverLost(e);
        }
    }
}

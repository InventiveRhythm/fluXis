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
        private bool selected; // so that the first mapset is selected by default

        private MapListEntryHeader header;
        private Container difficultyContainer;
        private FillFlowContainer<MapDifficultyEntry> difficultyFlow;

        public MapListEntry(SelectScreen screen, MapSet mapset, int index)
        {
            Screen = screen;
            this.mapset = mapset;
            this.index = index;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;

            InternalChildren = new Drawable[]
            {
                header = new MapListEntryHeader(this, mapset),
                difficultyContainer = new Container
                {
                    Masking = true,
                    RelativeSizeAxes = Axes.X,
                    Y = 75,
                    Padding = new MarginPadding(5),
                    Child = difficultyFlow = new FillFlowContainer<MapDifficultyEntry>
                    {
                        Direction = FillDirection.Vertical,
                        AutoSizeAxes = Axes.Y,
                        RelativeSizeAxes = Axes.X,
                        Spacing = new Vector2(0, 5)
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
                    select();
                else
                    deselect();
            }

            // kind of a hack to make sure the first mapset is selected by default
            if (Selected && difficultyContainer.Height == 0)
                select();

            selected = Selected;

            base.Update();
        }

        private void select()
        {
            header.SetDim(.2f);
            difficultyContainer.FadeIn(300, Easing.OutQuint)
                               .ResizeHeightTo(difficultyFlow.Height + 10, 300, Easing.OutQuint);
        }

        private void deselect()
        {
            header.SetDim(.4f);
            difficultyContainer.FadeOut(300, Easing.OutQuint)
                               .ResizeHeightTo(0, 300, Easing.OutQuint);
        }

        protected override bool OnClick(ClickEvent e)
        {
            if (selected)
                return false; // dont handle clicks when we already selected this mapset

            Screen.SelectMapSet(mapset);
            return true;
        }

        protected override void LoadComplete()
        {
            const float duration = 200;
            const float delay = 50;

            this.MoveToX(-100)
                .FadeOut()
                .Delay(delay * index)
                .FadeIn(duration)
                .MoveToX(0, duration, Easing.OutQuint);

            base.LoadComplete();
        }

        private class MapListEntryHeader : Container
        {
            private readonly MapListEntry parent;
            private readonly MapSet mapset;
            private Box dim;
            private DelayedLoadWrapper backgroundWrapper;

            public MapListEntryHeader(MapListEntry parent, MapSet mapset)
            {
                this.parent = parent;
                this.mapset = mapset;
            }

            [BackgroundDependencyLoader]
            private void load()
            {
                RelativeSizeAxes = Axes.X;
                Height = 75;
                CornerRadius = 10;
                Margin = new MarginPadding { Bottom = 5 };
                Masking = true;
                Children = new Drawable[]
                {
                    backgroundWrapper = new DelayedLoadWrapper(() => new MapBackground(mapset)
                    {
                        RelativeSizeAxes = Axes.Both,
                        FillMode = FillMode.Fill,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                    }, 100),
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
                                Font = new FontUsage("Quicksand", 32f, "Bold"),
                                Text = mapset.Title,
                                Anchor = Anchor.TopLeft,
                                Origin = Anchor.TopLeft,
                                Y = -2
                            },
                            new SpriteText
                            {
                                Font = new FontUsage("Quicksand", 28f, "SemiBold"),
                                Text = mapset.Artist,
                                Anchor = Anchor.TopLeft,
                                Origin = Anchor.TopLeft,
                                Y = 24
                            }
                        }
                    }
                };

                backgroundWrapper.DelayedLoadComplete += drawable => drawable.FadeInFromZero(200);
            }

            protected override bool OnHover(HoverEvent e)
            {
                dim.FadeTo(parent.selected ? .1f : .3f, 100);
                return base.OnHover(e);
            }

            protected override void OnHoverLost(HoverLostEvent e)
            {
                dim.FadeTo(parent.selected ? .2f : .4f, 300);
                base.OnHoverLost(e);
            }

            public void SetDim(float alpha)
            {
                dim.FadeTo(alpha, 100);
            }
        }
    }
}

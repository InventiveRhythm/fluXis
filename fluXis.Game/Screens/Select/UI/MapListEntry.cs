using fluXis.Game.Graphics.Background;
using fluXis.Game.Map;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;

namespace fluXis.Game.Screens.Select.UI
{
    public class MapListEntry : Container
    {
        private readonly SelectScreen screen;
        private readonly MapSet mapset;
        private readonly int index;

        public bool Selected => screen?.MapSet == mapset;
        private bool selected;

        private Box dim;

        public MapListEntry(SelectScreen screen, MapSet mapset, int index)
        {
            this.screen = screen;
            this.mapset = mapset;
            this.index = index;
        }

        [BackgroundDependencyLoader]
        private void load(BackgroundTextureStore backgrounds)
        {
            RelativeSizeAxes = Axes.X;
            Height = 75;
            Margin = new MarginPadding { Bottom = 5 };
            CornerRadius = 10;
            Masking = true;

            InternalChildren = new Drawable[]
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
                    Alpha = .2f
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
            };
        }

        protected override void Update()
        {
            if (Selected != selected)
            {
                if (Selected)
                {
                    dim.FadeOut(100);
                }
                else
                {
                    dim.FadeTo(.2f, 100);
                }
            }

            selected = Selected;

            base.Update();
        }

        protected override bool OnClick(ClickEvent e)
        {
            if (Selected)
            {
            }
            else
            {
                screen.SelectMapSet(mapset);
            }

            return base.OnClick(e);
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
            return base.OnHover(e);
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            base.OnHoverLost(e);
        }
    }
}

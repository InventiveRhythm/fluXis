using System.IO;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Map;
using fluXis.Game.Screens.Gameplay;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Screens;

namespace fluXis.Game.Screens.Select.UI
{
    public class MapListEntry : Container
    {
        private readonly SelectScreen screen;
        private readonly MapInfo map;
        private int index;

        public MapListEntry(SelectScreen screen, MapInfo map, int index)
        {
            this.screen = screen;
            this.map = map;
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
                    Texture = backgrounds.Get($"{map.MapsetID}{Path.DirectorySeparatorChar}{map.GetBackgroundFile()}"),
                    FillMode = FillMode.Fill,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre
                },
                new Box
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
                            Text = map.Metadata.Title,
                            Anchor = Anchor.TopLeft,
                            Origin = Anchor.TopLeft,
                            Y = -2
                        },
                        new SpriteText
                        {
                            Font = new FontUsage("Quicksand", 28f),
                            Text = map.Metadata.Artist,
                            Anchor = Anchor.TopLeft,
                            Origin = Anchor.TopLeft,
                            Y = 24
                        }
                    }
                }
            };
        }

        protected override bool OnClick(ClickEvent e)
        {
            screen.Backgrounds.AddBackgroundFromMap(map);
            screen.Push(new GameplayScreen(map));

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

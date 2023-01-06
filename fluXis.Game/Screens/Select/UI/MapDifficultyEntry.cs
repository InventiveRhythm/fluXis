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
    public class MapDifficultyEntry : Container
    {
        private readonly MapListEntry mapListEntry;
        private readonly MapInfo map;

        private SpriteText difficultyName;
        private SpriteText difficultyMapper;

        public MapDifficultyEntry(MapListEntry parentEntry, MapInfo map)
        {
            mapListEntry = parentEntry;
            this.map = map;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.X;
            Height = 40;
            CornerRadius = 5;
            Masking = true;

            InternalChildren = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Colour4.FromHex("#1a1a20")
                },
                difficultyName = new SpriteText
                {
                    Text = map.Metadata.Difficulty,
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Y = -1,
                    Margin = new MarginPadding { Left = 10 },
                    Font = new FontUsage("Quicksand", 22, "SemiBold")
                },
                difficultyMapper = new SpriteText
                {
                    Text = $"mapped by {map.Metadata.Mapper}",
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Y = -1,
                    Margin = new MarginPadding { Left = 10 },
                    Font = new FontUsage("Quicksand", 22)
                }
            };
        }

        protected override void LoadComplete()
        {
            difficultyMapper.X = difficultyName.DrawWidth + 3;
            base.LoadComplete();
        }

        protected override bool OnClick(ClickEvent e)
        {
            mapListEntry.Screen.MapInfo = map;
            mapListEntry.Screen.MenuAccept.Play();
            mapListEntry.Screen.Backgrounds.AddBackgroundFromMap(map);
            mapListEntry.Screen.Backgrounds.SwipeAnimation();
            mapListEntry.Screen.Push(new GameplayScreen(map));

            return base.OnClick(e);
        }
    }
}

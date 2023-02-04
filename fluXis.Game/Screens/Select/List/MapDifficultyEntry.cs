using fluXis.Game.Database.Maps;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;

namespace fluXis.Game.Screens.Select.List
{
    public partial class MapDifficultyEntry : Container
    {
        private readonly MapListEntry mapListEntry;
        private readonly RealmMap map;

        private SpriteText difficultyName;
        private SpriteText difficultyMapper;
        private Container backgroundContainer;
        private DiffKeyCount keyCount;

        private bool selected;
        private float glowAmount = 0f;

        public MapDifficultyEntry(MapListEntry parentEntry, RealmMap map)
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
            EdgeEffect = new EdgeEffectParameters
            {
                Type = EdgeEffectType.Glow,
                Colour = Colour4.Transparent,
                Radius = 5
            };

            InternalChildren = new Drawable[]
            {
                keyCount = new DiffKeyCount(map.KeyCount),
                backgroundContainer = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Masking = true,
                    CornerRadius = 5,
                    Child = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Colour4.FromHex("#1a1a20")
                    },
                },
                difficultyName = new SpriteText
                {
                    Text = map.Difficulty,
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
            backgroundContainer.Width = (backgroundContainer.RelativeToAbsoluteFactor.X - 40) / backgroundContainer.RelativeToAbsoluteFactor.X;
            base.LoadComplete();
        }

        protected override bool OnClick(ClickEvent e)
        {
            if (Equals(mapListEntry.Screen.MapInfo, map))
                mapListEntry.Screen.Accept();
            else
                mapListEntry.Screen.SelectMap(map);

            return base.OnClick(e);
        }

        protected override void Update()
        {
            bool newSelected = Equals(mapListEntry.Screen.MapInfo, map);

            if (selected != newSelected)
            {
                this.TransformTo(nameof(glowAmount), newSelected ? 1f : 0f, 200, Easing.OutQuint);
                selected = newSelected;
            }

            updateGlow();

            base.Update();
        }

        private void updateGlow()
        {
            EdgeEffectParameters glow = EdgeEffect;
            glow.Colour = keyCount.KeyColour.Opacity(glowAmount);
            EdgeEffect = glow;
        }
    }
}

using System.Collections.Generic;
using fluXis.Game.Map;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Graphics.Background
{
    public class BackgroundStack : CompositeDrawable
    {
        private readonly List<Background> scheduledBackgrounds = new List<Background>();
        private Container backgroundContainer;
        private MapInfo currentMap;

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.Both;

            InternalChildren = new Drawable[]
            {
                backgroundContainer = new Container
                {
                    RelativeSizeAxes = Axes.Both
                },
                new Box
                {
                    Colour = Colour4.Black,
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0.25f
                }
            };
        }

        protected override void Update()
        {
            while (scheduledBackgrounds.Count > 0)
            {
                var background = scheduledBackgrounds[0];
                scheduledBackgrounds.RemoveAt(0);
                backgroundContainer.Add(background);
            }

            while (backgroundContainer.Children.Count > 1)
            {
                if (backgroundContainer.Children[1].Alpha == 1)
                    backgroundContainer.Remove(backgroundContainer.Children[0], false);
                else
                    break;
            }

            base.Update();
        }

        public void AddBackgroundFromMap(MapInfo map)
        {
            if (map == null || map == currentMap)
                return;

            currentMap = map;
            scheduledBackgrounds.Add(new Background(map));
        }
    }
}

using System.Collections.Generic;
using System.IO;
using fluXis.Game.Map;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Graphics.Background
{
    public class BackgroundStack : CompositeDrawable
    {
        private readonly List<Background> scheduledBackgrounds = new List<Background>();
        private Container backgroundContainer;
        private string currentBackground;
        private Box swipeAnimation;

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
                },
                swipeAnimation = new Box
                {
                    Colour = Colour4.Black,
                    RelativeSizeAxes = Axes.Both,
                    RelativePositionAxes = Axes.Both,
                    Width = 1.1f,
                    Alpha = 0,
                    Shear = new Vector2(.1f, 0),
                    X = -1.1f
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
            var path = map == null ? "" : $"{map.MapsetID}{Path.DirectorySeparatorChar}{map.GetBackgroundFile()}";

            if (path == currentBackground)
                return;

            currentBackground = path;
            scheduledBackgrounds.Add(new Background(path));
        }

        public void SwipeAnimation()
        {
            const int duration = 1100;
            const int delay = 1200;
            const int fade_duration = 1500;

            swipeAnimation.MoveToX(-1.1f)
                          .FadeTo(.5f)
                          .MoveToX(0, duration, Easing.OutQuint)
                          .Delay(delay)
                          .FadeOut(fade_duration);
        }
    }
}

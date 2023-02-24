using System.Collections.Generic;
using fluXis.Game.Configuration;
using fluXis.Game.Database;
using fluXis.Game.Database.Maps;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Graphics.Background
{
    public partial class BackgroundStack : CompositeDrawable
    {
        private readonly List<Background> scheduledBackgrounds = new();
        private readonly Container backgroundContainer;
        private readonly Box swipeAnimation;
        private readonly Box backgroundDim;
        private string currentBackground;

        private Bindable<float> backgroundDimBindable;
        private Bindable<float> backgroundBlurBindable;

        public BackgroundStack()
        {
            RelativeSizeAxes = Axes.Both;

            InternalChildren = new Drawable[]
            {
                backgroundContainer = new Container
                {
                    RelativeSizeAxes = Axes.Both
                },
                backgroundDim = new Box
                {
                    Colour = Colour4.Black,
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0
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

        [BackgroundDependencyLoader]
        private void load(FluXisConfig config)
        {
            backgroundDimBindable = config.GetBindable<float>(FluXisSetting.BackgroundDim);
            backgroundBlurBindable = config.GetBindable<float>(FluXisSetting.BackgroundBlur);
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

            // for some reason using the bindable directly doesn't work
            if (backgroundDimBindable.Value != backgroundDim.Alpha)
                backgroundDim.Alpha = backgroundDimBindable.Value;

            foreach (var background in backgroundContainer)
            {
                if (background is Background b)
                {
                    if (b.Blur != backgroundBlurBindable.Value)
                        b.Blur = backgroundBlurBindable.Value;
                }
            }

            base.Update();
        }

        public void AddBackgroundFromMap(RealmMap map)
        {
            RealmFile file = map?.MapSet?.GetFile(map.Metadata?.Background);
            string path = file?.GetPath() ?? "";

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

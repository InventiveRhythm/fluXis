using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Screens.Gameplay.Ruleset
{
    public class Stage : Container
    {
        private const int lane_margin = 20;

        private Box background;
        private Box borderLeft;
        private Box borderRight;

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.Both;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;

            AddRangeInternal(new[]
            {
                background = new Box
                {
                    RelativeSizeAxes = Axes.Y,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Colour = new Colour4(0, 0, 0, .4f)
                },
                borderLeft = new Box
                {
                    RelativeSizeAxes = Axes.Y,
                    Height = 0f,
                    Width = 5,
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopRight,
                    Colour = new Colour4(255, 255, 255, .4f),
                    X = -(Receptor.SIZE.X * 2 + lane_margin)
                },
                borderRight = new Box
                {
                    RelativeSizeAxes = Axes.Y,
                    Height = 0f,
                    Width = 5,
                    Anchor = Anchor.BottomCentre,
                    Origin = Anchor.BottomLeft,
                    Colour = new Colour4(255, 255, 255, .4f),
                    X = Receptor.SIZE.X * 2 + lane_margin
                }
            });
        }

        protected override void LoadComplete()
        {
            const float duration = 800f;

            background.ResizeWidthTo(0).ResizeWidthTo(Receptor.SIZE.X * 4 + lane_margin * 2, duration, Easing.OutQuint);
            borderLeft.ResizeHeightTo(0).ResizeHeightTo(1f, duration, Easing.OutQuint);
            borderRight.ResizeHeightTo(0).ResizeHeightTo(1f, duration, Easing.OutQuint);

            base.LoadComplete();
        }

        public void OnExit()
        {
            const float duration = 800f;

            borderLeft.Anchor = Anchor.BottomCentre;
            borderLeft.Origin = Anchor.BottomLeft;
            borderLeft.ResizeHeightTo(0f, duration, Easing.OutQuint);

            borderRight.Anchor = Anchor.TopCentre;
            borderRight.Origin = Anchor.TopRight;
            borderRight.ResizeHeightTo(0f, duration, Easing.OutQuint);

            background.ResizeWidthTo(0, duration, Easing.OutQuint);
        }
    }
}

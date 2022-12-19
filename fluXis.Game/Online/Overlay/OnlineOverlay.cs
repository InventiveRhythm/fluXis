using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Online.Overlay
{
    public class OnlineOverlay : CompositeDrawable
    {
        private bool visible;

        private Box background;
        private FillFlowContainer content;

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.Both;

            InternalChildren = new Drawable[]
            {
                background = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Colour4.Black,
                    Alpha = 0
                },
                content = new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Direction = FillDirection.Horizontal,
                    Alpha = 0,
                    Scale = new Vector2(1.1f),
                    Padding = new MarginPadding(20),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Children = new Drawable[]
                    {
                        new OnlineSidebar(),
                        new OverlayChat()
                    }
                }
            };
        }

        public void ToggleVisibility()
        {
            if (visible)
            {
                background.FadeOut(200);
                content.ScaleTo(1.1f, 200, Easing.InQuint).FadeOut(200);
            }
            else
            {
                background.FadeTo(.4f, 200);
                content.ScaleTo(1f, 400, Easing.OutQuint).FadeIn(200);
            }

            visible = !visible;
        }

        protected override bool OnClick(ClickEvent e)
        {
            // only handle if we're visible
            return visible;
        }

        protected override bool OnHover(HoverEvent e)
        {
            // only handle if we're visible
            return visible;
        }
    }
}

using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Menus;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.UserInterface;
using osuTK;

namespace fluXis.Graphics.UserInterface.Form;

public partial class FormDropdown<T>
{
    private partial class InnerMenu : Dropdown<T>.DropdownMenu
    {
        private float marginTop
        {
            get => Margin.Top;
            set => Margin = new MarginPadding { Top = value };
        }

        public InnerMenu()
        {
            MaskingContainer.CornerRadius = 8;
            BackgroundColour = Theme.Background3;
            ScrollbarVisible = false;
        }

        protected override Menu CreateSubMenu() => new FluXisMenu(Direction.Vertical);
        protected override DrawableDropdownMenuItem CreateDrawableDropdownMenuItem(MenuItem item) => new InnerMenuItem(item);
        protected override ScrollContainer<Drawable> CreateScrollContainer(Direction direction) => new FluXisScrollContainer(direction) { ScrollbarVisible = true };

        protected override void UpdateSize(Vector2 newSize)
        {
            if (Direction != Direction.Vertical)
                return;

            Width = newSize.X;
            this.ResizeHeightTo(newSize.Y, 300, Easing.OutQuint);
            this.TransformTo(nameof(marginTop), newSize.Y > 0 ? 8f : 0f, 300, Easing.OutQuint);
        }

        protected override void AnimateOpen() => this.FadeInFromZero(Styling.TRANSITION_FADE);
        protected override void AnimateClose() => this.FadeOut(Styling.TRANSITION_FADE);

        private partial class InnerMenuItem : DrawableDropdownMenuItem
        {
            public InnerMenuItem(MenuItem item)
                : base(item)
            {
                BackgroundColour = Theme.Background3;
                BackgroundColourSelected = Theme.Background3;
                BackgroundColourHover = Theme.Background4;

                ForegroundColour = ForegroundColourHover = ForegroundColourSelected = Theme.Text;

                Foreground.AutoSizeAxes = Axes.X;
                Foreground.Height = 32;
                Foreground.Padding = new MarginPadding { Horizontal = 12 };
            }

            public override void SetFlowDirection(Direction direction)
            {
                RelativeSizeAxes = Axes.X;
                AutoSizeAxes = Axes.None;
                Height = 32;
            }

            protected override Drawable CreateContent() => new FluXisSpriteText
            {
                WebFontSize = 12,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Colour = Colour4.White
            };
        }
    }
}

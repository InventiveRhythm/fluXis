using System;
using fluXis.Game.Audio;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Text;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Game.Graphics.UserInterface.Menus;

public partial class FluXisDropdown<T> : Dropdown<T>
{
    protected override DropdownHeader CreateHeader() => new FluXisDropdownHeader();
    protected override DropdownMenu CreateMenu() => new FluXisDropdownMenu();

    protected partial class FluXisDropdownHeader : DropdownHeader
    {
        protected FluXisSpriteText LabelText { get; }
        protected SpriteIcon Icon { get; }

        protected override LocalisableString Label
        {
            get => LabelText.Text;
            set => LabelText.Text = value;
        }

        public FluXisDropdownHeader()
        {
            AutoSizeAxes = Axes.None;
            Height = 40;
            Foreground.Padding = new MarginPadding { Horizontal = 14 };
            CornerRadius = 10;
            BackgroundColour = FluXisColors.Background2;
            BackgroundColourHover = FluXisColors.Background4;

            Children = new Drawable[]
            {
                LabelText = new FluXisSpriteText
                {
                    WebFontSize = 16,
                    AlwaysPresent = true,
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft
                },
                Icon = new SpriteIcon
                {
                    Icon = FontAwesome6.Solid.AngleDown,
                    Anchor = Anchor.CentreRight,
                    Origin = Anchor.CentreRight,
                    Size = new Vector2(16)
                }
            };
        }

        protected void UpdateOpenState(bool state)
        {
            var vec = new Vector2(1, state ? -1 : 1);
            Icon.ScaleTo(vec, 400, Easing.OutQuint);
        }

        protected override DropdownSearchBar CreateSearchBar() => new FluXisDropdownSearchBar(UpdateOpenState);

        protected override bool OnHover(HoverEvent e)
        {
            base.OnHover(e);
            return true;
        }

        protected partial class FluXisDropdownSearchBar : DropdownSearchBar
        {
            protected virtual int SidePadding => 14;
            protected virtual float FontSize => FluXisSpriteText.GetWebFontSize(16);

            private Action<bool> stateAction { get; }

            public FluXisDropdownSearchBar(Action<bool> stateAction)
            {
                this.stateAction = stateAction;
            }

            protected override TextBox CreateTextBox() => new FluXisTextBox
            {
                RelativeSizeAxes = Axes.X,
                Height = 40,
                SidePadding = SidePadding,
                FontSize = FontSize,
                PlaceholderText = "Search",
                OnFocusAction = () => stateAction?.Invoke(true),
                OnFocusLostAction = () => stateAction?.Invoke(false)
            };

            protected override void PopIn() => this.FadeIn(200);
            protected override void PopOut() => this.FadeOut(200);
        }
    }

    protected partial class FluXisDropdownMenu : DropdownMenu
    {
        [Resolved]
        private UISamples samples { get; set; }

        private bool justLoaded = true;

        private float marginTop
        {
            get => Margin.Top;
            set => Margin = new MarginPadding { Top = value };
        }

        public FluXisDropdownMenu()
        {
            MaskingContainer.CornerRadius = 10;
            BackgroundColour = FluXisColors.Background2;
            Margin = new MarginPadding { Top = 8 };
            ScrollbarVisible = false;
        }

        protected override void UpdateSize(Vector2 newSize)
        {
            if (Direction != Direction.Vertical)
                return;

            Width = newSize.X;
            this.ResizeHeightTo(newSize.Y, 300, Easing.OutQuint);
            this.TransformTo(nameof(marginTop), newSize.Y > 0 ? 8f : 0f, 300, Easing.OutQuint);
        }

        protected override Menu CreateSubMenu() => new FluXisMenu(Direction.Vertical);
        protected override DrawableDropdownMenuItem CreateDrawableDropdownMenuItem(MenuItem item) => new DrawableFluXisDropdownMenuItem(item);
        protected override ScrollContainer<Drawable> CreateScrollContainer(Direction direction) => new FluXisScrollContainer(direction) { ScrollbarVisible = true };

        protected override void AnimateClose()
        {
            this.FadeOut(200);

            if (!justLoaded) // AnimateClose is called on load, so we don't want to play the sound then
                samples.Dropdown(true);

            justLoaded = false;
        }

        protected override void AnimateOpen()
        {
            this.FadeIn(200);
            samples.Dropdown(false);
        }

        protected partial class DrawableFluXisDropdownMenuItem : DrawableDropdownMenuItem
        {
            public DrawableFluXisDropdownMenuItem(MenuItem item)
                : base(item)
            {
                BackgroundColour = FluXisColors.Background2;
                BackgroundColourHover = FluXisColors.Background4;
                BackgroundColourSelected = FluXisColors.Background3;

                Foreground.AutoSizeAxes = Axes.X;
                Foreground.Height = 30;
                Foreground.Padding = new MarginPadding { Horizontal = 14 };
            }

            public override void SetFlowDirection(Direction direction)
            {
                RelativeSizeAxes = Axes.X;
                AutoSizeAxes = Axes.None;
                Height = 30;
            }

            protected override Drawable CreateContent()
            {
                return new FluXisSpriteText
                {
                    WebFontSize = 12,
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                };
            }
        }
    }
}

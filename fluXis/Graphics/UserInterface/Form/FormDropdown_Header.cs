using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Graphics.UserInterface.Form;

public partial class FormDropdown<T>
{
    private partial class InnerHeader : DropdownHeader
    {
        private readonly InnerDropdown parent;

        protected override LocalisableString Label { get => text.Text; set => text.Text = value; }

        private readonly ForcedHeightText title;
        private readonly ForcedHeightText text;
        private readonly FluXisSpriteIcon icon;
        private InnerSearchBar search;

        public InnerHeader(InnerDropdown parent)
        {
            this.parent = parent;

            AutoSizeAxes = Axes.None;
            Height = HEIGHT;
            BackgroundColour = Theme.Background3;
            BackgroundColourHover = Theme.Background3;
            Foreground.Padding = new MarginPadding(PADDING);
            CornerRadius = RADIUS;

            Child = new GridContainer
            {
                RelativeSizeAxes = Axes.X,
                Height = 40,
                ColumnDimensions = [new Dimension(), new Dimension(GridSizeMode.Absolute, 8), new Dimension(GridSizeMode.Absolute, 40)],
                Content = new[]
                {
                    new[]
                    {
                        new FillFlowContainer
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Direction = FillDirection.Vertical,
                            Spacing = new Vector2(INNER_GAP),
                            Children =
                            [
                                title = new ForcedHeightText(true)
                                {
                                    RelativeSizeAxes = Axes.X,
                                    WebFontSize = 14,
                                    Height = 16,
                                    Colour = Theme.TextVariant
                                },
                                text = new ForcedHeightText(true)
                                {
                                    RelativeSizeAxes = Axes.X,
                                    WebFontSize = 18,
                                    Height = 22
                                }
                            ]
                        },
                        Empty(),
                        icon = new FluXisSpriteIcon
                        {
                            Size = new Vector2(20),
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Icon = Phosphor.Bold.CaretDown
                        }
                    }
                }
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            title.Text = parent.ParentDropdown.Label;
        }

        protected override DropdownSearchBar CreateSearchBar() => search = new InnerSearchBar(v => icon.ScaleTo(new Vector2(1, v ? -1 : 1), 300, Easing.OutQuint));

        protected override void Update()
        {
            base.Update();

            search.RelativeSizeAxes = Axes.None;
            search.Position = ToLocalSpace(text.ScreenSpaceDrawQuad).TopLeft;
            search.Size = text.DrawSize;
        }
    }
}

using fluXis.Audio;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Screens.Select.Search.Dropdown;

public partial class SearchDropdownStatus : Container
{
    [Resolved]
    private SearchFilters filters { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 30;

        InternalChildren = new Drawable[]
        {
            new FluXisSpriteText
            {
                Text = "Status",
                FontSize = 24,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                X = 5
            },
            new FillFlowContainer
            {
                AutoSizeAxes = Axes.X,
                RelativeSizeAxes = Axes.Y,
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
                Direction = FillDirection.Horizontal,
                Spacing = new Vector2(5),
                Children = new StatusChip[]
                {
                    new() { Status = -2, Text = "Local", DropdownItem = this },
                    new() { Status = 0, Text = "Unsubmitted", DropdownItem = this },
                    new() { Status = 1, Text = "Pending", DropdownItem = this },
                    new() { Status = 2, Text = "Impure", DropdownItem = this },
                    new() { Status = 3, Text = "Pure", DropdownItem = this },
                }
            }
        };
    }

    private bool onStatusClick(StatusChip chip)
    {
        if (filters.Status.Contains(chip.Status))
            filters.Status.Remove(chip.Status);
        else
            filters.Status.Add(chip.Status);

        filters.OnChange.Invoke();
        return filters.Status.Contains(chip.Status);
    }

    private partial class StatusChip : Container
    {
        public int Status { get; init; }
        public string Text { get; init; }
        public SearchDropdownStatus DropdownItem { get; init; }

        [Resolved]
        private UISamples samples { get; set; }

        private Box colorBox;
        private HoverLayer hoverBox;
        private FluXisSpriteText text;

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.Y;
            Width = 110;
            CornerRadius = 10;
            Masking = true;

            var color = FluXisColors.GetStatusColor(Status);

            InternalChildren = new Drawable[]
            {
                colorBox = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = color,
                    Alpha = 0
                },
                hoverBox = new HoverLayer(),
                text = new FluXisSpriteText
                {
                    Text = Text.ToUpper(),
                    FontSize = 16,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre
                }
            };
        }

        protected override bool OnClick(ClickEvent e)
        {
            samples.Click();

            if (DropdownItem.onStatusClick(this))
            {
                colorBox.FadeIn(200);
                text.FadeColour(FluXisColors.IsBright(colorBox.Colour) ? FluXisColors.TextDark : FluXisColors.Text, 200);
            }
            else
            {
                colorBox.FadeOut(200);
                text.FadeColour(FluXisColors.Text, 200);
            }

            return true;
        }

        protected override bool OnHover(HoverEvent e)
        {
            hoverBox.Show();
            samples.Hover();
            return true;
        }

        protected override void OnHoverLost(HoverLostEvent e) => hoverBox.Hide();
    }
}

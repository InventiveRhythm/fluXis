using fluXis.Audio;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Interaction;
using fluXis.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input;
using osu.Framework.Input.Events;
using osuTK;
using System.Linq;

namespace fluXis.Screens.Select.Search.Dropdown;

public partial class SearchDropdownStatus : Container
{
    private readonly InputManager input;

    [Resolved]
    private SearchFilters filters { get; set; }

    private FillFlowContainer chipFlow;

    private readonly int[] statuses = { -2, 0, 1, 2, 3 };

    public SearchDropdownStatus(InputManager input)
    {
        this.input = input;
    }

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
            chipFlow = new FillFlowContainer
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

    private void onStatusClick(int status)
    {
        bool isCtrlPressed = input.CurrentState.Keyboard.ControlPressed;

        if (filters.Status.Count == statuses.Length)
            filters.Status.Clear();

        if (isCtrlPressed && filters.Status.Count == 0)
        {
            filters.Status.Clear();
            filters.Status.AddRange(statuses);
        }

        if (!filters.Status.Remove(status))
            filters.Status.Add(status);

        filters.OnChange.Invoke();
        
        foreach (var chip in chipFlow.Children.OfType<StatusChip>())
            chip.UpdateSelection();
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

            var color = Theme.GetStatusColor(Status);

            InternalChildren = new Drawable[]
            {
                colorBox = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = color,
                },
                hoverBox = new HoverLayer(),
                text = new FluXisSpriteText
                {
                    Text = Text.ToUpper(),
                    FontSize = 16,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Colour = Theme.IsBright(color) ? Theme.TextDark : Theme.Text
                }
            };
        }

        public void UpdateSelection()
        {
            bool isSelected = DropdownItem.filters.Status.Count == 0 || DropdownItem.filters.Status.Contains(Status);

            colorBox.FadeTo(isSelected ? 1 : 0, 200);
            text.FadeColour(isSelected ? (Theme.IsBright(colorBox.Colour) ? Theme.TextDark : Theme.Text) : Theme.Text, 200);
        }

        protected override bool OnClick(ClickEvent e)
        {
            samples.Click();
            DropdownItem.onStatusClick(Status);
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
using System;
using System.Collections.Generic;
using fluXis.Audio;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Buttons;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Interaction;
using fluXis.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input;
using osu.Framework.Input.Events;

namespace fluXis.Screens.Select.Search.Dropdown;

public partial class SearchDropdownStatus : FluXisFilterButtonsBase<int>
{
    [Resolved]
    private SearchFilters filters { get; set; }

    protected override int[] Values => new[] { -2, 0, 1, 2, 3 };
    protected override string Label => "Status";
    protected override float FontSize { get; set; } = 24;
    protected override List<int> FilterList => filters.Status;
    public override int[] DefaultFilter { get; set; } = Array.Empty<int>();
    public override bool ResetWhenFull { get; set; } = true;

    private static readonly Dictionary<int, string> statuses = new()
    {
        { -2, "Local" },
        { 0, "Unsubmitted" },
        { 1, "Pending" },
        { 2, "Impure" },
        { 3, "Pure" }
    };

    public SearchDropdownStatus(InputManager input) 
        : base(input)
    {
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        OnFilterChanged = filters.OnChange.Invoke;
    }

    protected override Drawable CreateButton(int status) => new StatusChip(status, statuses[status], this);

    private partial class StatusChip : Container, ISelectableButton<int>
    {
        private readonly SearchDropdownStatus dropdownItem;
        private readonly int status;

        public Action<ISelectableButton<int>> OnRightClick { get; set; }

        [Resolved]
        private UISamples samples { get; set; }

        private Box colorBox;
        private HoverLayer hoverBox;
        private FlashLayer flash;
        private FluXisSpriteText text;

        public StatusChip(int status, string text, SearchDropdownStatus dropdownItem)
        {
            this.status = status;
            this.dropdownItem = dropdownItem;

            InternalChild = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    colorBox = new Box { RelativeSizeAxes = Axes.Both },
                    hoverBox = new HoverLayer(),
                    flash = new FlashLayer(),
                    this.text = new FluXisSpriteText
                    {
                        Text = text.ToUpper(),
                        FontSize = 16,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre
                    }
                }
            };
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.Y;
            Width = 110;
            CornerRadius = 10;
            Masking = true;

            var color = Theme.GetStatusColor(status);
            colorBox.Colour = color;
            flash.Colour = color.Lighten(.8f);
            text.Colour = Theme.IsBright(color) ? Theme.TextDark : Theme.Text;
        }

        public void UpdateSelection()
        {
            bool isSelected = (dropdownItem.FilterList.Count == 0 && dropdownItem.DefaultFilter.Length == 0) || dropdownItem.FilterList.Contains(status);

            colorBox.FadeTo(isSelected ? 1 : 0, 200);
            text.FadeColour(isSelected ? (Theme.IsBright(colorBox.Colour) ? Theme.TextDark : Theme.Text) : Theme.Text, 200);
        }

        protected override bool OnMouseDown(MouseDownEvent e)
        {
            if (e.Button == osuTK.Input.MouseButton.Right)
            {
                OnRightClick?.Invoke(this);
            }
            else
            {
                samples.Click();
                dropdownItem.OnValueClick(status);
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

        public void Flash() => flash.Show();
    }
}
using System.Collections.Generic;
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

namespace fluXis.Screens.Select.Search.Dropdown;

public partial class SearchDropdownKeymode : FluXisFilterButtonsBase<int>
{
    [Resolved]
    private SearchFilters filters  { get; set; }

    protected override int[] Values => new[] { 4, 5, 6, 7, 8 };
    protected override string Label => "Keymode";
    protected override float FontSize { get; set; } = 24;
    protected override List<int> FilterList => filters.Keymodes;
    public override int[] DefaultFilter { get; set; } = System.Array.Empty<int>();

    public SearchDropdownKeymode(InputManager input) 
        : base(input)
    {
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        OnFilterChanged = filters.OnChange.Invoke;
    }

    protected override Drawable CreateButton(int keymode) => new KeymodeButton(keymode, this);

    private partial class KeymodeButton : Container, ISelectableButton<int>
    {
        private readonly SearchDropdownKeymode dropdownItem;
        private readonly int keymode;

        [Resolved]
        private UISamples samples { get; set; }

        private Box background;
        private HoverLayer hoverBox;
        private FlashLayer flash;
        private FluXisSpriteText text;

        public KeymodeButton(int keymode, SearchDropdownKeymode dropdownItem)
        {
            this.keymode = keymode;
            this.dropdownItem = dropdownItem;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Width = 50;
            RelativeSizeAxes = Axes.Y;
            Masking = true;
            CornerRadius = 5;

            var color = Theme.GetKeyCountColor(keymode);

            InternalChildren = new Drawable[]
            {
                background = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = color,
                },
                hoverBox = new HoverLayer(),
                flash = new FlashLayer(),
                text = new FluXisSpriteText
                {
                    Text = $"{keymode}K",
                    FontSize = 16,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Colour = Theme.IsBright(color) ? Theme.TextDark : Theme.Text
                }
            };
        }

        public void UpdateSelection()
        {
            bool isSelected = (dropdownItem.FilterList.Count == 0 && dropdownItem.DefaultFilter.Length == 0) || dropdownItem.FilterList.Contains(keymode);

            background.FadeTo(isSelected ? 1 : 0, 200);
            text.FadeColour(isSelected ? (Theme.IsBright(background.Colour) ? Theme.TextDark : Theme.Text) : Theme.Text, 200);
        }

        protected override bool OnClick(ClickEvent e)
        {
            samples.Click();
            dropdownItem.OnValueClick(keymode);
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

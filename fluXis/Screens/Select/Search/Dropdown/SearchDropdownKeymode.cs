using System.Linq;
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

namespace fluXis.Screens.Select.Search.Dropdown;

public partial class SearchDropdownKeymode : CompositeDrawable
{
    private readonly InputManager input;

    [Resolved]
    private SearchFilters filters { get; set; }

    private FillFlowContainer buttonFlow;

    private readonly int[] keymodes = { 4, 5, 6, 7, 8 };

    public SearchDropdownKeymode(InputManager input)
    {
        this.input = input;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 30;
        Padding = new MarginPadding { Horizontal = 5 };

        InternalChildren = new Drawable[]
        {
            new FluXisSpriteText
            {
                Text = "Keymode",
                FontSize = 24,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft
            },
            buttonFlow = new FillFlowContainer
            {
                AutoSizeAxes = Axes.X,
                RelativeSizeAxes = Axes.Y,
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
                Direction = FillDirection.Horizontal,
                Spacing = new Vector2(5),
                ChildrenEnumerable = keymodes.Select(i => new KeymodeButton(i, this))
            }
        };
    }

    private void onKeymodeClick(int keymode)
    {
        bool isCtrlPressed = input.CurrentState.Keyboard.ControlPressed;

        if (filters.Keymodes.Count == keymodes.Length)
            filters.Keymodes.Clear();

        if (isCtrlPressed && filters.Keymodes.Count == 0)
        {
            filters.Keymodes.Clear();
            filters.Keymodes.AddRange(keymodes);
        }

        if (!filters.Keymodes.Remove(keymode))
            filters.Keymodes.Add(keymode);

        filters.OnChange.Invoke();
        
        foreach (var button in buttonFlow.Children.OfType<KeymodeButton>())
            button.UpdateSelection();
    }

    private partial class KeymodeButton : Container
    {
        public SearchDropdownKeymode DropdownItem { get; init; }
        private int keymode { get; }

        [Resolved]
        private UISamples samples { get; set; }

        private Box background;
        private HoverLayer hoverBox;
        private FluXisSpriteText text;

        public KeymodeButton(int keymode, SearchDropdownKeymode DropdownItem)
        {
            this.keymode = keymode;
            this.DropdownItem = DropdownItem;
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
            bool isSelected = DropdownItem.filters.Keymodes.Count == 0 || DropdownItem.filters.Keymodes.Contains(keymode);

            background.FadeTo(isSelected ? 1 : 0, 200);
            text.FadeColour(isSelected ? (Theme.IsBright(background.Colour) ? Theme.TextDark : Theme.Text) : Theme.Text, 200);
        }

        protected override bool OnClick(ClickEvent e)
        {
            samples.Click();
            DropdownItem.onKeymodeClick(keymode);
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
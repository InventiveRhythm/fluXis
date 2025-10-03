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
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Screens.Select.Search.Dropdown;

public partial class SearchDropdownKeymode : CompositeDrawable
{
    [Resolved]
    private SearchFilters filters { get; set; }

    private readonly int[] keymodes = { 4, 5, 6, 7, 8 };

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
            new FillFlowContainer
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

    private bool onKeymodeClick(int keymode)
    {
        if (!filters.Keymodes.Remove(keymode))
            filters.Keymodes.Add(keymode);

        filters.OnChange.Invoke();
        return filters.Keymodes.Contains(keymode);
    }

    private partial class KeymodeButton : Container
    {
        private SearchDropdownKeymode parent { get; }
        private int keymode { get; }

        [Resolved]
        private UISamples samples { get; set; }

        private Box background;
        private HoverLayer hoverBox;
        private FluXisSpriteText text;

        public KeymodeButton(int keymode, SearchDropdownKeymode parent)
        {
            this.keymode = keymode;
            this.parent = parent;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Width = 50;
            RelativeSizeAxes = Axes.Y;
            Masking = true;
            CornerRadius = 5;

            InternalChildren = new Drawable[]
            {
                background = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Theme.GetKeyCountColor(keymode),
                    Alpha = 0
                },
                hoverBox = new HoverLayer(),
                text = new FluXisSpriteText
                {
                    Text = $"{keymode}K",
                    FontSize = 16,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre
                }
            };
        }

        protected override bool OnClick(ClickEvent e)
        {
            samples.Click();

            if (parent.onKeymodeClick(keymode))
            {
                background.FadeIn(200);
                text.FadeColour(Theme.IsBright(background.Colour) ? Theme.TextDark : Theme.Text, 200);
            }
            else
            {
                background.FadeOut(200);
                text.FadeColour(Theme.Text, 200);
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
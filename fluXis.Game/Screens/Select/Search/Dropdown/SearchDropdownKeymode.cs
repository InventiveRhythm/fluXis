using System.Linq;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Screens.Select.Search.Dropdown;

public partial class SearchDropdownKeymode : CompositeDrawable
{
    [Resolved]
    private SearchFilters filters { get; set; }

    private Bindable<int> keymodeBindable { get; } = new();

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
                ChildrenEnumerable = keymodes.Select(i => new KeymodeButton(i, keymodeBindable))
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        keymodeBindable.BindValueChanged(e => filters.KeyMode = e.NewValue);
    }

    private partial class KeymodeButton : ClickableContainer
    {
        private Bindable<int> bind { get; }
        private int keymode { get; }

        private Box background;
        private FluXisSpriteText text;

        public KeymodeButton(int keymode, Bindable<int> bind)
        {
            this.keymode = keymode;
            this.bind = bind;

            Action = () =>
            {
                if (bind.Value == keymode)
                {
                    bind.Value = 0;
                    return;
                }

                bind.Value = keymode;
            };
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
                    Colour = FluXisColors.GetKeyColor(keymode)
                },
                text = new FluXisSpriteText
                {
                    Text = $"{keymode}K",
                    FontSize = 16,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Colour = FluXisColors.TextDark
                }
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            bind.BindValueChanged(updateKeymode, true);
        }

        private void updateKeymode(ValueChangedEvent<int> e)
        {
            var enable = e.NewValue == keymode || e.NewValue == 0;

            background.FadeTo(enable ? 1 : 0, 200);
            text.FadeColour(enable ? FluXisColors.TextDark : FluXisColors.Text, 200);
        }
    }
}

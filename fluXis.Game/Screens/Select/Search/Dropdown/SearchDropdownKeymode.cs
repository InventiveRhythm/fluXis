using System.Linq;
using fluXis.Game.Graphics.Drawables;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Game.Screens.Select.Search.Dropdown;

public partial class SearchDropdownKeymode : CompositeDrawable
{
    [Resolved]
    private SearchFilters filters { get; init; }

    private readonly int[] keymodes = { 4, 5, 6, 7, 8 };
    private int currentKeymode;

    private FillFlowContainer tictacs;

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
            tictacs = new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
                Direction = FillDirection.Horizontal,
                Spacing = new Vector2(5),
                ChildrenEnumerable = keymodes.Select(keymode => new ClickableContainer
                {
                    AutoSizeAxes = Axes.Both,
                    Action = () => setKeymode(keymode),
                    Child = new TicTac(20)
                })
            }
        };
    }

    private void setKeymode(int mode)
    {
        if (currentKeymode == mode)
        {
            tictacs.Colour = Colour4.White;
            currentKeymode = 0;

            foreach (var drawable in tictacs)
                drawable.Alpha = 1;
        }
        else
        {
            tictacs.Colour = FluXisColors.GetKeyColor(mode);
            currentKeymode = mode;

            var i = 0;

            foreach (var drawable in tictacs)
            {
                var m = keymodes[i];
                drawable.Alpha = m <= mode ? 1 : 0.5f;
                i++;
            }
        }

        filters.KeyMode = currentKeymode;
    }
}

using System;
using fluXis.Graphics;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Overlay.User.Header;

public partial class HeaderPlacementChip : CircularContainer
{
    public Func<Drawable> CreateIcon { get; set; } = () => new Container();
    public int Placement { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.X;
        Height = 48;
        Masking = true;
        EdgeEffect = Styling.ShadowSmall;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Theme.Background2
            },
            new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Direction = FillDirection.Horizontal,
                Padding = new MarginPadding { Horizontal = 20 },
                Spacing = new Vector2(10),
                Children = new[]
                {
                    CreateIcon(),
                    new FluXisSpriteText
                    {
                        Text = $"#{(Placement == 0 ? "-" : Placement.ToString())}",
                        WebFontSize = 16
                    }
                }
            }
        };
    }
}

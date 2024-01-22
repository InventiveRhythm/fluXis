using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Game.Overlay.User.Header;

public partial class HeaderRoleChip : Container
{
    public int RoleId { get; init; } = 0;

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.X;
        Height = 30;
        CornerRadius = 15;
        Masking = true;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background2.Opacity(.5f)
            },
            new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Direction = FillDirection.Horizontal,
                Spacing = new Vector2(5),
                Padding = new MarginPadding(10),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Children = new Drawable[]
                {
                    new SpriteIcon
                    {
                        Icon = getIcon(),
                        Size = new Vector2(18),
                        Colour = FluXisColors.GetRoleColor(RoleId)
                    },
                    new FluXisSpriteText
                    {
                        Text = getText(),
                        WebFontSize = 14,
                        Colour = FluXisColors.GetRoleColor(RoleId)
                    }
                }
            }
        };
    }

    private IconUsage getIcon()
    {
        return RoleId switch
        {
            1 => FontAwesome6.Solid.Star,
            2 => FontAwesome6.Solid.Diamond, // we dont have the icon that the website uses
            3 => FontAwesome6.Solid.ShieldHalved,
            4 => FontAwesome6.Solid.UserShield,
            5 => FontAwesome6.Solid.UserAstronaut,
            _ => FontAwesome6.Solid.User
        };
    }

    private string getText()
    {
        return RoleId switch
        {
            1 => "Featured Artist",
            2 => "Purifier",
            3 => "Moderator",
            4 => "Admin",
            5 => "fluxel",
            _ => "User"
        };
    }
}

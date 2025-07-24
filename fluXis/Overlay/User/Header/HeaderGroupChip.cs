using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Online.API.Models.Groups;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Overlay.User.Header;

public partial class HeaderGroupChip : CircularContainer
{
    public bool AccentAsBackground { get; init; } = false;
    private APIGroup group { get; }

    public HeaderGroupChip(APIGroup group)
    {
        this.group = group;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.X;
        Height = 32;
        Masking = true;

        var color = Colour4.FromHex(group.Color);

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = AccentAsBackground ? color : Theme.Background2.Opacity(.5f)
            },
            new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Direction = FillDirection.Horizontal,
                Spacing = new Vector2(6),
                Padding = new MarginPadding(12),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Colour = AccentAsBackground ? Theme.Background2 : color,
                Children = new Drawable[]
                {
                    new FluXisSpriteIcon
                    {
                        Icon = getIcon(),
                        Size = new Vector2(14),
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft
                    },
                    new FluXisSpriteText
                    {
                        Text = group.Name,
                        WebFontSize = 14,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft
                    }
                }
            }
        };
    }

    private IconUsage getIcon()
    {
        return group.ID switch
        {
            "fa" => FontAwesome6.Solid.Star,
            "purifier" => FontAwesome6.Solid.Diamond, // we don't have the icon that the website uses
            "moderators" => FontAwesome6.Solid.ShieldHalved,
            "dev" => FontAwesome6.Solid.UserShield,
            "bot" => FontAwesome6.Solid.UserAstronaut,
            "supporter" => FontAwesome6.Solid.Heart,
            _ => FontAwesome6.Solid.User
        };
    }
}

using fluXis.Graphics.Sprites.Icons;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Overlay.Auth.UI;

public partial class AuthOverlayIconButton : AuthOverlayButton
{
    private readonly IconUsage icon;

    public AuthOverlayIconButton(IconUsage icon, string text)
        : base(text)
    {
        this.icon = icon;
    }

    protected override Drawable CreateContent() => new FillFlowContainer
    {
        RelativeSizeAxes = Axes.Both,
        Direction = FillDirection.Horizontal,
        Spacing = new Vector2(8),
        Children =
        [
            new FluXisSpriteIcon
            {
                Size = new Vector2(20),
                Icon = icon,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Colour = TextColor
            },
            TextSprite = CreateTextSprite()
        ]
    };
}

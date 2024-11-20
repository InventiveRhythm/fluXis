using System;
using fluXis.Game.Audio;
using fluXis.Game.Graphics.Sprites;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Game.Screens.Menu.UI;

public partial class MenuLinkButton : Container, IHasTooltip
{
    public LocalisableString TooltipText => Text;

    public string Text { get; set; } = string.Empty;
    public Action Action { get; set; }
    public IconUsage Icon { set => icon.Icon = value; }

    [Resolved]
    private UISamples samples { get; set; }

    private readonly SpriteIcon icon;

    public MenuLinkButton()
    {
        Size = new Vector2(40);
        Alpha = .6f;

        Child = icon = new FluXisSpriteIcon
        {
            RelativeSizeAxes = Axes.Both,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Shadow = true
        };
    }

    protected override bool OnHover(HoverEvent e)
    {
        this.FadeTo(1f, 50);
        samples.Hover();
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        this.FadeTo(.6f, 200);
    }

    protected override bool OnClick(ClickEvent e)
    {
        Action?.Invoke();
        samples.Click();
        return true;
    }
}

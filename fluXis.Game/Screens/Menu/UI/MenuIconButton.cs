using System;
using fluXis.Game.Overlay.Mouse;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Screens.Menu.UI;

public partial class MenuIconButton : Container, IHasTooltip
{
    public string Tooltip => Text;

    public string Text { get; set; } = string.Empty;
    public Action Action { get; set; }
    public IconUsage Icon { set => icon.Icon = value; }

    private readonly SpriteIcon icon;

    private Sample sampleClick;
    private Sample sampleHover;

    public MenuIconButton()
    {
        Size = new Vector2(40);
        Alpha = .6f;

        Child = icon = new SpriteIcon
        {
            RelativeSizeAxes = Axes.Both,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre
        };
    }

    [BackgroundDependencyLoader]
    private void load(ISampleStore samples)
    {
        sampleClick = samples.Get("UI/accept.ogg");
        sampleHover = samples.Get("UI/scroll.ogg");
    }

    protected override bool OnHover(HoverEvent e)
    {
        this.FadeTo(2f, 200);
        sampleHover?.Play();
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        this.FadeTo(.6f, 200);
    }

    protected override bool OnClick(ClickEvent e)
    {
        Action?.Invoke();
        sampleClick?.Play();
        return true;
    }
}

using System;
using fluXis.Game.Graphics;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;

namespace fluXis.Game.Screens.Select.Footer;

public partial class SelectFooterButton : Container
{
    public string Text { get => text.Text.ToString(); set => text.Text = value; }

    public Action Action { get; set; }

    private readonly SpriteText text;
    private readonly Box box;

    public SelectFooterButton()
    {
        RelativeSizeAxes = Axes.Y;
        Width = 110;
        Anchor = Anchor.CentreLeft;
        Origin = Anchor.CentreLeft;

        AddRangeInternal(new Drawable[]
        {
            box = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Alpha = 0
            },
            text = new SpriteText
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Font = FluXisFont.Default(24)
            }
        });
    }

    protected override bool OnClick(ClickEvent e)
    {
        box.FadeTo(.4f).FadeTo(.2f, 200);
        Action?.Invoke();
        return base.OnClick(e);
    }

    protected override bool OnHover(HoverEvent e)
    {
        box.FadeTo(.2f, 200);
        return base.OnHover(e);
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        box.FadeTo(0, 200);
        base.OnHoverLost(e);
    }
}

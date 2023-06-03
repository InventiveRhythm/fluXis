using System;
using fluXis.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Screens.Menu.UI;

public partial class SmallMenuButton : Container
{
    public Action Action { get; set; }
    public string Text { set => text.Text = value; }
    public IconUsage Icon { set => icon.Icon = value; }

    private readonly Box hover;
    private readonly SpriteIcon icon;
    private readonly FluXisSpriteText text;

    private Sample sampleClick;
    private Sample sampleHover;

    public SmallMenuButton()
    {
        Height = 40;
        CornerRadius = 5;
        Masking = true;
        EdgeEffect = new EdgeEffectParameters
        {
            Type = EdgeEffectType.Shadow,
            Colour = Colour4.Black.Opacity(.2f),
            Radius = 5,
            Offset = new Vector2(0, 1)
        };

        Children = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background
            },
            hover = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Alpha = 0
            },
            icon = new SpriteIcon
            {
                Size = new Vector2(20),
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Margin = new MarginPadding { Left = 10 },
                Shadow = true
            },
            text = new FluXisSpriteText
            {
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Margin = new MarginPadding { Left = 40 },
                Shadow = true
            }
        };
    }

    [BackgroundDependencyLoader]
    private void load(ISampleStore samples)
    {
        sampleClick = samples.Get("UI/accept.mp3");
        sampleHover = samples.Get("UI/scroll.mp3");
    }

    protected override bool OnHover(HoverEvent e)
    {
        hover.FadeTo(.2f, 200);
        sampleHover?.Play();
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        hover.FadeOut(200);
    }

    protected override bool OnClick(ClickEvent e)
    {
        Action?.Invoke();
        sampleClick?.Play();
        return true;
    }
}

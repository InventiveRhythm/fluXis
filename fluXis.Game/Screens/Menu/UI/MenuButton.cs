using System;
using fluXis.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Screens.Menu.UI;

public partial class MenuButton : Container
{
    public Action Action { get; set; }
    public string Text { get; set; }
    public string Description { get; set; }
    public IconUsage Icon { get; set; }
    public float ShearAmount => Width / 250f * .2f + ShearAdjust;
    public float ShearAdjust { get; set; }

    public bool Enabled
    {
        get => enabled;
        set
        {
            enabled = value;
            Alpha = value ? 1 : .5f;
        }
    }

    private bool enabled = true;

    private Box hover;

    private Sample sampleClick;
    private Sample sampleHover;

    [BackgroundDependencyLoader]
    private void load()
    {
        Height = 60;
        CornerRadius = 10;
        Masking = true;
        Shear = new Vector2(-ShearAmount, 0);

        Children = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background2
            },
            hover = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Alpha = 0
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Shear = new Vector2(ShearAmount, 0),
                Padding = new MarginPadding { Horizontal = Shear.X * -50 },
                Children = new Drawable[]
                {
                    new SpriteIcon
                    {
                        Size = new Vector2(30),
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Margin = new MarginPadding { Left = 10 },
                        Shadow = true,
                        Icon = Icon
                    },
                    new FluXisSpriteText
                    {
                        FontSize = 30,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.BottomLeft,
                        Margin = new MarginPadding { Left = 50 },
                        Y = 8,
                        Shadow = true,
                        Text = Text
                    },
                    new FluXisSpriteText
                    {
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.TopLeft,
                        Margin = new MarginPadding { Left = 50 },
                        Colour = FluXisColors.Text2,
                        Shadow = true,
                        Text = Description
                    }
                }
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
        if (!Enabled)
            return false;

        hover.FadeTo(.2f, 200);
        sampleHover?.Play();
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        if (!Enabled)
            return;

        hover.FadeOut(200);
    }

    protected override bool OnClick(ClickEvent e)
    {
        if (!Enabled)
            return false;

        Action?.Invoke();
        sampleClick?.Play();

        return true;
    }
}

using System;
using fluXis.Game.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Screens.Menu.UI;

public partial class SmallMenuButton : Container
{
    public Action Action { get; set; }
    public IconUsage Icon { get; set; }

    private float shearAmount => Width / 100f * .2f;

    private Container hover;

    private Sample sampleClick;
    private Sample sampleHover;

    [BackgroundDependencyLoader]
    private void load(ISampleStore samples)
    {
        sampleClick = samples.Get("UI/accept.mp3");
        sampleHover = samples.Get("UI/scroll.mp3");

        Height = 60;
        CornerRadius = 10;
        Masking = true;

        Children = new Drawable[]
        {
            new Container // Background
            {
                RelativeSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Width = .5f,
                        Colour = FluXisColors.Background2
                    },
                    new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Width = .7f,
                        Anchor = Anchor.BottomRight,
                        Origin = Anchor.BottomRight,
                        Shear = new Vector2(-shearAmount, 0),
                        CornerRadius = 10,
                        Masking = true,
                        Child = new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = FluXisColors.Background2
                        }
                    }
                }
            },
            hover = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Alpha = 0,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Width = .5f
                    },
                    new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Width = .7f,
                        Anchor = Anchor.BottomRight,
                        Origin = Anchor.BottomRight,
                        Shear = new Vector2(-shearAmount, 0),
                        CornerRadius = 10,
                        Masking = true,
                        Child = new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Width = 0.714f,
                            Shear = new Vector2(shearAmount, 0),
                            Anchor = Anchor.BottomRight,
                            Origin = Anchor.BottomRight
                        }
                    }
                }
            },
            new SpriteIcon
            {
                Size = new Vector2(20),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                X = -5,
                Icon = Icon
            }
        };
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

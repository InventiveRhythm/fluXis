using System;
using fluXis.Game.Audio;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Graphics;

namespace fluXis.Game.Screens.Menu.UI;

public partial class MenuButton : Container
{
    public Action Action { get; set; }
    public string Text { get; set; }
    public string Description { get; set; }
    public IconUsage Icon { get; set; }

    [Resolved]
    private UISamples samples { get; set; }

    private Container content;
    private Box hover;
    private Box flash;

    [BackgroundDependencyLoader]
    private void load()
    {
        Height = 60;

        Child = content = new Container
        {
            RelativeSizeAxes = Axes.Both,
            CornerRadius = 10,
            Masking = true,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            EdgeEffect = new EdgeEffectParameters
            {
                Type = EdgeEffectType.Shadow,
                Colour = Color4.Black.Opacity(.25f),
                Radius = 5
            },
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
                flash = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0
                },
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Shear = new Vector2(.2f, 0),
                    Padding = new MarginPadding { Left = 20 },
                    Children = new Drawable[]
                    {
                        new SpriteIcon
                        {
                            Size = new Vector2(30),
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Shadow = true,
                            Icon = Icon
                        },
                        new FluXisSpriteText
                        {
                            FontSize = 30,
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.BottomLeft,
                            X = 40,
                            Y = 8,
                            Shadow = true,
                            Text = Text
                        },
                        new FluXisSpriteText
                        {
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.TopLeft,
                            X = 40,
                            Colour = FluXisColors.Text2,
                            Shadow = true,
                            Text = Description
                        }
                    }
                }
            }
        };
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        content.ScaleTo(.95f, 1000, Easing.OutQuint);
        return true;
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        content.ScaleTo(1, 1000, Easing.OutElastic);
    }

    protected override bool OnHover(HoverEvent e)
    {
        hover.FadeTo(.2f, 50);
        samples.Hover();
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        hover.FadeOut(200);
    }

    protected override bool OnClick(ClickEvent e)
    {
        flash.FadeOutFromOne(1000, Easing.OutQuint);
        Action?.Invoke();
        samples.Click();

        return true;
    }
}

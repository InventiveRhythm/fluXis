using fluXis.Game.Audio;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Localization;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Game.Screens.Menu.UI;

public partial class MenuButton : ClickableContainer, IHasTooltip
{
    public LocalisableString TooltipText => Enabled.Value ? "" : LocalizationStrings.General.LoginToUse;

    public LocalisableString Text { get; init; }
    public LocalisableString Description { get; init; }
    public IconUsage Icon { get; init; }

    [Resolved]
    private UISamples samples { get; set; }

    private Container content;
    private Box hover;
    private Box flash;
    private Box dim;

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
            EdgeEffect = FluXisStyles.ShadowSmall,
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
                },
                dim = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = FluXisColors.Background1
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        Enabled.BindValueChanged(e => dim.FadeTo(e.NewValue ? 0 : .8f, 200), true);
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        if (Enabled.Value)
            content.ScaleTo(.95f, 1000, Easing.OutQuint);

        return true;
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        content.ScaleTo(1, 1000, Easing.OutElastic);
    }

    protected override bool OnHover(HoverEvent e)
    {
        if (Enabled.Value)
        {
            hover.FadeTo(.2f, 50);
            samples.Hover();
        }

        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        hover.FadeOut(200);
    }

    protected override bool OnClick(ClickEvent e)
    {
        samples.Click(!Enabled.Value);

        if (Enabled.Value)
            flash.FadeOutFromOne(1000, Easing.OutQuint);

        return base.OnClick(e);
    }
}

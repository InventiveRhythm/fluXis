using System;
using System.Linq;
using fluXis.Audio;
using fluXis.Graphics;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Interaction;
using fluXis.Localization;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osuTK.Graphics;
using osuTK.Input;

namespace fluXis.Screens.Menu.UI.Buttons;

public abstract partial class MenuButtonBase : CompositeDrawable, IHasTooltip
{
    public LocalisableString TooltipText => Enabled.Value ? "" : LocalizationStrings.General.LoginToUse;

    public override bool HandlePositionalInput => IsVisible;

    public const float SHEAR_AMOUNT = 0.1f;

    public IconUsage Icon { get; init; } = FontAwesome6.Solid.Question;
    public LocalisableString Text { get; init; } = "Default Text";
    public virtual LocalisableString Description { get; set; } = string.Empty;
    public Action Action { get; init; }
    public Key[] Keys { get; init; }
    public JoystickButton GamepadButton { get; init; }
    public BindableBool Enabled { get; } = new(true);

    // used for animation delay
    public int Row { get; init; } = 1;
    public int Column { get; init; } = 1;

    public bool IsVisible => content.Alpha > 0;

    private float animationDelay
    {
        get
        {
            var count = 0;
            count += Column - 1;
            count += Row - 1;
            return count * FluXisScreen.ENTER_DELAY;
        }
    }

    [Resolved]
    protected UISamples Samples { get; private set; }

    protected Box Background { get; private set; }

    private Container content;
    private HoverLayer hover;
    private FlashLayer flash;
    private Box dim;

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChild = new Container // wrapper so we can freely animate the button
        {
            RelativeSizeAxes = Axes.Both,
            Child = content = new Container
            {
                RelativeSizeAxes = Axes.Both,
                CornerRadius = 10,
                Masking = true,
                Alpha = 0,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                EdgeEffect = FluXisStyles.ShadowMedium,
                ChildrenEnumerable = new Drawable[]
                {
                    Background = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = FluXisColors.Background2
                    },
                    hover = new HoverLayer()
                }.Concat(CreateContent()).Concat(new Drawable[]
                {
                    flash = new FlashLayer(),
                    dim = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Color4.Black.Opacity(0.5f),
                        Alpha = 0
                    }
                })
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        Enabled.BindValueChanged(enabled =>
        {
            dim.FadeTo(enabled.NewValue ? 0 : 1, 300, Easing.OutQuint);
        }, true);
    }

    protected abstract Drawable[] CreateContent();

    protected override bool OnClick(ClickEvent e)
    {
        if (!IsVisible || !Enabled.Value)
            return false;

        Samples.Click(!Enabled.Value);
        flash.Show();

        Action?.Invoke();
        return true;
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        if (!Enabled.Value || !IsVisible)
            return false;

        content.ScaleTo(0.95f, 1000, Easing.OutQuint);
        return true;
    }

    protected override void OnMouseUp(MouseUpEvent e)
    {
        content.ScaleTo(1, 1000, Easing.OutElasticHalf);
    }

    protected override bool OnHover(HoverEvent e)
    {
        if (!Enabled.Value || !IsVisible)
            return false;

        Samples.Hover();
        hover.Show();
        return true;
    }

    protected override void OnHoverLost(HoverLostEvent e)
    {
        hover.Hide();
    }

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        if (Keys is null || !Keys.Contains(e.Key))
            return false;

        return TriggerClick();
    }

    protected override bool OnJoystickPress(JoystickPressEvent e)
    {
        return e.Button == GamepadButton && TriggerClick();
    }

    public override void Show()
    {
        content.Delay(animationDelay).MoveToX(100)
               .MoveToX(0, FluXisScreen.MOVE_DURATION, Easing.OutQuint)
               .FadeInFromZero(FluXisScreen.FADE_DURATION);
    }

    public override void Hide()
    {
        content.Delay(animationDelay).FadeOut(FluXisScreen.FADE_DURATION)
               .MoveToX(-100, FluXisScreen.MOVE_DURATION, Easing.OutQuint);
    }
}

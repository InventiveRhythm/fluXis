using System.Collections.Generic;
using fluXis.Game.Graphics.Gamepad;
using fluXis.Game.Graphics.UserInterface.Buttons;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Input;
using fluXis.Game.Screens;
using osu.Framework.Allocation;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;

namespace fluXis.Game.Graphics.UserInterface.Footer;

#nullable enable

public abstract partial class Footer : CompositeDrawable
{
    public Container<FooterButton> ButtonContainer { get; private set; } = null!;

    private Container background = null!;
    private CornerButton? leftButton;
    private CornerButton? rightButton;
    private Container content = null!;
    private GamepadTooltipBar? gamepadTooltips;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 60;
        Anchor = Origin = Anchor.BottomLeft;

        InternalChildren = new Drawable[]
        {
            background = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Masking = true,
                Y = 80,
                EdgeEffect = FluXisStyles.ShadowMediumNoOffset,
                Child = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = FluXisColors.Background2
                }
            }
        };

        var backgroundContent = CreateBackgroundContent();

        if (backgroundContent != null)
            AddInternal(backgroundContent);

        content = new Container
        {
            RelativeSizeAxes = Axes.Both,
            Padding = new MarginPadding { Horizontal = 10 }
        };

        leftButton = CreateLeftButton();
        rightButton = CreateRightButton();

        if (leftButton != null)
            content.Add(leftButton);

        content.Add(ButtonContainer = new Container<FooterButton>
        {
            RelativeSizeAxes = Axes.Y,
            AutoSizeAxes = Axes.X,
            Y = 100,
            Anchor = Anchor.BottomLeft,
            Origin = Anchor.BottomLeft,
            Padding = new MarginPadding { Left = 300 },
            ChildrenEnumerable = CreateButtons()
        });

        if (rightButton != null)
            content.Add(rightButton);

        AddInternal(content);

        var foregroundContent = CreateForegroundContent();

        if (foregroundContent != null)
            AddInternal(foregroundContent);

        gamepadTooltips = CreateGamepadTooltips();

        if (gamepadTooltips != null)
            AddInternal(gamepadTooltips);
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        GamepadHandler.OnGamepadStatusChanged += updateGamepadStatus;
        updateGamepadStatus(GamepadHandler.GamepadConnected);
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);
        GamepadHandler.OnGamepadStatusChanged -= updateGamepadStatus;
    }

    private void updateGamepadStatus(bool status)
    {
        if (gamepadTooltips == null)
            return;

        content.FadeTo(status ? 0 : 1);
        gamepadTooltips.FadeTo(status ? 1 : 0);
    }

    protected override bool OnHover(HoverEvent e) => true;
    protected override bool OnClick(ClickEvent e) => true;

    public override void Show()
    {
        leftButton?.Show();
        rightButton?.Show();
        background.MoveToY(0, FluXisScreen.MOVE_DURATION, Easing.OutQuint);
        ButtonContainer.MoveToY(0);
        ButtonContainer.ForEach(b => b.Show());
    }

    public override void Hide()
    {
        leftButton?.Hide();
        rightButton?.Hide();
        background.MoveToY(80, FluXisScreen.MOVE_DURATION, Easing.OutQuint);
        ButtonContainer.MoveToY(100, FluXisScreen.MOVE_DURATION, Easing.OutQuint);
    }

    protected abstract CornerButton? CreateLeftButton();
    protected abstract CornerButton? CreateRightButton();

    protected virtual Drawable? CreateBackgroundContent() => null;
    protected virtual Drawable? CreateForegroundContent() => null;
    protected virtual GamepadTooltipBar? CreateGamepadTooltips() => null;

    protected abstract IEnumerable<FooterButton> CreateButtons();
}

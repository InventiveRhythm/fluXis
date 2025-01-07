using System.Collections.Generic;
using fluXis.Graphics.Gamepad;
using fluXis.Graphics.UserInterface.Buttons;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Input;
using fluXis.Screens;
using osu.Framework.Allocation;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;

namespace fluXis.Graphics.UserInterface.Footer;

#nullable enable

public abstract partial class Footer : CompositeDrawable
{
    public Container<FooterButton> ButtonContainer { get; private set; } = null!;

    private Container background = null!;
    public CornerButton? LeftButton { get; private set; }
    public CornerButton? RightButton { get; private set; }
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

        LeftButton = CreateLeftButton();
        RightButton = CreateRightButton();

        if (LeftButton != null)
            content.Add(LeftButton);

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

        if (RightButton != null)
            content.Add(RightButton);

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

    protected override void Update()
    {
        base.Update();

        var x = 0f;

        foreach (var button in ButtonContainer)
        {
            button.X = x;
            x += button.DrawWidth + 10;
        }
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
        LeftButton?.Show();
        RightButton?.Show();
        background.MoveToY(0, FluXisScreen.MOVE_DURATION, Easing.OutQuint);
        ButtonContainer.MoveToY(0);
        ButtonContainer.ForEach(b => b.Show());
    }

    public override void Hide()
    {
        LeftButton?.Hide();
        RightButton?.Hide();
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

using System;
using fluXis.Game.Graphics;
using fluXis.Game.Input;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Overlay.Login.UI;
using fluXis.Game.Overlay.Register;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Graphics;

namespace fluXis.Game.Overlay.Login;

public partial class LoginOverlay : Container, IKeyBindingHandler<FluXisKeybind>
{
    [Resolved]
    private RegisterOverlay register { get; set; }

    [Resolved]
    private Fluxel fluxel { get; set; }

    private LoginContent loginContainer;
    private FluXisSpriteText loadingText;

    private LoginTextBox username;
    private LoginTextBox password;

    private ClickableContainer content;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        Alpha = 0;

        InternalChildren = new Drawable[]
        {
            new ClickableContainer
            {
                RelativeSizeAxes = Axes.Both,
                Action = Hide,
                Child = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color4.Black,
                    Alpha = .5f
                }
            },
            content = new ClickableContainer
            {
                Anchor = Anchor.TopRight,
                Origin = Anchor.TopRight,
                Margin = new MarginPadding { Top = 30, Right = 80 },
                Width = 300,
                Height = 180,
                CornerRadius = 10,
                Masking = true,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = FluXisColors.Background2
                    },
                    loginContainer = new LoginContent
                    {
                        Padding = new MarginPadding { Top = 30 },
                        Children = new Drawable[]
                        {
                            username = new LoginTextBox(false, "Username")
                            {
                                TabbableContentContainer = loginContainer
                            },
                            password = new LoginTextBox(true, "Password")
                            {
                                TabbableContentContainer = loginContainer
                            },
                            new LoginButton("Create new...", true)
                            {
                                Action = _ =>
                                {
                                    Hide();
                                    register.Show();
                                }
                            },
                            new LoginButton("Login!") { Action = login }
                        }
                    },
                    loadingText = new FluXisSpriteText
                    {
                        Text = "",
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        FontSize = 24
                    }
                }
            }
        };

        fluxel.OnStatusChanged += updateStatus;
        updateStatus(fluxel.Status);
    }

    private void updateStatus(ConnectionStatus status)
    {
        Schedule(() =>
        {
            switch (status)
            {
                case ConnectionStatus.Online:
                    Hide();
                    break;

                case ConnectionStatus.Connecting:
                    switchToLoading();
                    loadingText.Text = "Logging in...";
                    loadingText.FadeIn(200);
                    break;

                case ConnectionStatus.Failing:
                    setLoadingText("Failed to connect to server...", () => { });
                    break;

                case ConnectionStatus.Offline:
                    switchToLogin();
                    break;
            }
        });
    }

    private void switchToLogin()
    {
        loadingText.FadeOut(200)
                   .OnComplete(_ => loginContainer.FadeIn(200));
    }

    private void switchToLoading()
    {
        loginContainer.FadeOut(200);
    }

    private void login(LoginButton button)
    {
        if (string.IsNullOrEmpty(username.Text))
        {
            username.NotifyError();
            return;
        }

        if (string.IsNullOrEmpty(password.Text))
        {
            password.NotifyError();
            return;
        }

        switchToLoading();

        if (fluxel.Status != ConnectionStatus.Online)
            fluxel.Login(username.Text, password.Text);
        else
            setLoadingText("You are already logged in???", switchToLogin);
    }

    private void setLoadingText(string text, Action onComplete)
    {
        Schedule(() =>
        {
            loadingText.FadeOut(200).OnComplete(_ =>
            {
                loadingText.Text = text;
                loadingText.FadeIn(200)
                           .Then(400).FadeIn()
                           .OnComplete(_ => onComplete?.Invoke());
            });
        });
    }

    public override void Show()
    {
        if (fluxel.Status == ConnectionStatus.Online)
            return;

        this.FadeIn(200);
        content.ResizeHeightTo(160, 400, Easing.OutQuint);
    }

    public override void Hide()
    {
        this.FadeOut(200);
        content.ResizeHeightTo(0, 200, Easing.InQuint);
    }

    protected override bool OnHover(HoverEvent e) => true;
    protected override bool OnDragStart(DragStartEvent e) => true;
    protected override bool OnKeyDown(KeyDownEvent e) => true;
    protected override bool OnScroll(ScrollEvent e) => true;

    public bool OnPressed(KeyBindingPressEvent<FluXisKeybind> e)
    {
        switch (e.Action)
        {
            case FluXisKeybind.Back:
                Hide();
                break;
        }

        return true;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisKeybind> e) { }

    private partial class LoginContent : FillFlowContainer
    {
        public LoginContent()
        {
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;
            Padding = new MarginPadding(20) { Top = 40 };
            Spacing = new Vector2(10, 10);
        }
    }
}

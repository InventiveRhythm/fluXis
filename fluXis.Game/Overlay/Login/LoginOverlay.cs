using fluXis.Game.Configuration;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface;
using fluXis.Game.Graphics.UserInterface.Buttons;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Text;
using fluXis.Game.Input;
using fluXis.Game.Online.Fluxel;
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

public partial class LoginOverlay : Container, IKeyBindingHandler<FluXisGlobalKeybind>
{
    [Resolved]
    private RegisterOverlay register { get; set; }

    [Resolved]
    private FluxelClient fluxel { get; set; }

    private FillFlowContainer loginContainer;

    private FluXisTextBox username;
    private FluXisTextBox password;

    private ClickableContainer content;
    private FluXisSpriteText errorText;
    private Container loadingContainer;

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config)
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
                Width = 340,
                AutoSizeAxes = Axes.Y,
                Anchor = Anchor.TopRight,
                Origin = Anchor.TopRight,
                Margin = new MarginPadding { Top = -20, Right = -20 },
                CornerRadius = 20,
                Masking = true,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = FluXisColors.Background1
                    },
                    loginContainer = new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Direction = FillDirection.Vertical,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        Spacing = new Vector2(10),
                        Padding = new MarginPadding { Top = 80, Bottom = 10, Left = 10, Right = 30 },
                        Children = new Drawable[]
                        {
                            errorText = new FluXisSpriteText
                            {
                                Text = "error",
                                Colour = FluXisColors.Red,
                                Alpha = 0
                            },
                            username = new FluXisTextBox
                            {
                                TabbableContentContainer = this,
                                PlaceholderText = "Username",
                                Text = config.Get<string>(FluXisSetting.Username),
                                RelativeSizeAxes = Axes.X,
                                Height = 30
                            },
                            password = new FluXisTextBox
                            {
                                TabbableContentContainer = this,
                                PlaceholderText = "Password",
                                IsPassword = true,
                                RelativeSizeAxes = Axes.X,
                                Height = 30
                            },
                            new FluXisButton
                            {
                                Text = "Login!",
                                Color = FluXisColors.Accent,
                                FontSize = 16,
                                RelativeSizeAxes = Axes.X,
                                Height = 30,
                                Action = login
                            },
                            new FluXisButton
                            {
                                Text = "Create new...",
                                RelativeSizeAxes = Axes.X,
                                Height = 30,
                                Color = FluXisColors.Accent2,
                                FontSize = 16,
                                Action = () =>
                                {
                                    Hide();
                                    register.Show();
                                }
                            }
                        }
                    },
                    loadingContainer = new FullInputBlockingContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        Alpha = 0,
                        Children = new Drawable[]
                        {
                            new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Colour = Color4.Black,
                                Alpha = .5f
                            },
                            new LoadingIcon
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Size = new Vector2(25)
                            }
                        }
                    }
                }
            }
        };

        password.OnCommit += (_, _) => login();

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
                    loadingContainer.FadeIn(200);
                    break;

                case ConnectionStatus.Failing:
                    errorText.Text = fluxel.LastError;
                    errorText.Alpha = 1;
                    loadingContainer.FadeOut(200);
                    break;

                case ConnectionStatus.Offline:
                    loadingContainer.FadeOut(200);
                    break;
            }
        });
    }

    private void login()
    {
        errorText.Alpha = 0;

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

        if (fluxel.Status != ConnectionStatus.Online)
            fluxel.Login(username.Text, password.Text);
    }

    public override void Show()
    {
        if (fluxel.Status == ConnectionStatus.Online)
            return;

        this.FadeIn(200);
        content.MoveToY(0, 400, Easing.OutQuint);
        Schedule(() => GetContainingInputManager().ChangeFocus(string.IsNullOrEmpty(username.Text) ? username : password));
    }

    public override void Hide()
    {
        this.FadeOut(200);
        content.MoveToY(-20, 400, Easing.OutQuint);
    }

    protected override bool OnHover(HoverEvent e) => true;
    protected override bool OnDragStart(DragStartEvent e) => true;
    protected override bool OnKeyDown(KeyDownEvent e) => true;
    protected override bool OnScroll(ScrollEvent e) => true;

    public bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        switch (e.Action)
        {
            case FluXisGlobalKeybind.Back:
                Hide();
                break;
        }

        return true;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisGlobalKeybind> e) { }
}

using fluXis.Game.Audio;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface;
using fluXis.Game.Graphics.UserInterface.Buttons;
using fluXis.Game.Graphics.UserInterface.Panel;
using fluXis.Game.Graphics.UserInterface.Text;
using fluXis.Game.Input;
using fluXis.Game.Online.API.Models.Account;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Overlay.Notifications;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Graphics;

namespace fluXis.Game.Overlay.Register;

public partial class RegisterOverlay : Container, IKeyBindingHandler<FluXisGlobalKeybind>
{
    [Resolved]
    private NotificationManager notifications { get; set; }

    [Resolved]
    private Fluxel fluxel { get; set; }

    [Resolved]
    private UISamples samples { get; set; }

    private FillFlowContainer entry;
    private FillFlowContainer form;
    private ClickableContainer loadingOverlay;

    private FluXisTextBox username;
    private FluXisTextBox password;
    private FluXisTextBox email;

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
            new ClickableContainer
            {
                Width = 600,
                Height = 282,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Masking = true,
                CornerRadius = 10,
                Children = new Drawable[]
                {
                    new PanelBackground(),
                    entry = new FillFlowContainer
                    {
                        AutoSizeAxes = Axes.Both,
                        Direction = FillDirection.Vertical,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Children = new Drawable[]
                        {
                            new FluXisSpriteText
                            {
                                Text = "Registration",
                                Anchor = Anchor.TopCentre,
                                Origin = Anchor.TopCentre,
                                FontSize = 40
                            },
                            new FluXisSpriteText
                            {
                                Text = "Enter the world of fluXis!",
                                Anchor = Anchor.TopCentre,
                                Origin = Anchor.TopCentre
                            },
                            new Container
                            {
                                Height = 40
                            },
                            new FluXisButton
                            {
                                Width = 200,
                                Height = 40,
                                Text = "Continue",
                                Action = () =>
                                {
                                    entry.FadeOut(200);
                                    form.FadeIn(200);
                                }
                            }
                        }
                    },
                    form = new FillFlowContainer
                    {
                        AutoSizeAxes = Axes.Both,
                        Direction = FillDirection.Vertical,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Alpha = 0,
                        Children = new Drawable[]
                        {
                            username = new FluXisTextBox
                            {
                                Width = 560,
                                Height = 40,
                                Anchor = Anchor.TopLeft,
                                Origin = Anchor.TopLeft,
                                PlaceholderText = "Username"
                            },
                            new FluXisSpriteText
                            {
                                Text = "Your public name everyone will know you by.",
                                Anchor = Anchor.TopLeft,
                                Origin = Anchor.TopLeft,
                                RelativeSizeAxes = Axes.X,
                                FontSize = 14,
                                Margin = new MarginPadding { Left = 5 }
                            },
                            new Container
                            {
                                Height = 10
                            },
                            email = new FluXisTextBox
                            {
                                Width = 560,
                                Height = 40,
                                Anchor = Anchor.TopLeft,
                                Origin = Anchor.TopLeft,
                                PlaceholderText = "Email"
                            },
                            new FluXisSpriteText
                            {
                                Text = "Used for account verification or when you forget your password. No one will see this.",
                                Anchor = Anchor.TopLeft,
                                Origin = Anchor.TopLeft,
                                RelativeSizeAxes = Axes.X,
                                FontSize = 14,
                                Margin = new MarginPadding { Left = 5 }
                            },
                            new Container
                            {
                                Height = 10
                            },
                            password = new FluXisTextBox
                            {
                                Width = 560,
                                Height = 40,
                                Anchor = Anchor.TopLeft,
                                Origin = Anchor.TopLeft,
                                PlaceholderText = "Password",
                                IsPassword = true
                            },
                            new FluXisSpriteText
                            {
                                Text = "Must be at least 8 characters long. Choose something unique and hard to guess.",
                                Anchor = Anchor.TopLeft,
                                Origin = Anchor.TopLeft,
                                RelativeSizeAxes = Axes.X,
                                FontSize = 14,
                                Margin = new MarginPadding { Left = 5 }
                            },
                            new Container
                            {
                                Height = 20
                            },
                            new FluXisButton
                            {
                                Width = 200,
                                Height = 40,
                                Anchor = Anchor.TopCentre,
                                Origin = Anchor.TopCentre,
                                Text = "Create Account",
                                Action = register
                            }
                        }
                    },
                    loadingOverlay = new ClickableContainer
                    {
                        Alpha = 0,
                        RelativeSizeAxes = Axes.Both,
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
                                Size = new Vector2(50)
                            }
                        }
                    }
                }
            }
        };

        fluxel.RegisterListener<APIRegisterResponse>(EventType.Register, onRegister);
        fluxel.OnStatusChanged += onStatusChanged;
    }

    private void onStatusChanged(ConnectionStatus status)
    {
        Schedule(() =>
        {
            switch (status)
            {
                case ConnectionStatus.Online:
                    loadingOverlay.FadeOut(200);
                    Hide();
                    break;

                case ConnectionStatus.Connecting:
                    loadingOverlay.FadeIn(200);
                    break;

                case ConnectionStatus.Offline:
                case ConnectionStatus.Failing:
                    loadingOverlay.FadeOut(200);
                    break;
            }
        });
    }

    private void register()
    {
        if (fluxel.Status == ConnectionStatus.Online)
        {
            notifications.SendError("Already logged in!");
            return;
        }

        if (string.IsNullOrEmpty(username.Text))
        {
            username.NotifyError();
            return;
        }

        if (string.IsNullOrEmpty(email.Text))
        {
            email.NotifyError();
            return;
        }

        if (string.IsNullOrEmpty(password.Text) || password.Text.Length < 8)
        {
            password.NotifyError();
            return;
        }

        fluxel.Register(username.Text, password.Text, email.Text);
    }

    private void onRegister(FluxelResponse<APIRegisterResponse> response)
    {
        if (response.Status != 200) notifications.SendError("Failed to Register!", response.Message);
    }

    public override void Show()
    {
        entry.FadeIn();
        form.FadeOut();
        this.FadeIn(200);
        samples.Overlay(false);
    }

    public override void Hide()
    {
        this.FadeOut(200);
        samples.Overlay(true);
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

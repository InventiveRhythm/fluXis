using fluXis.Game.Audio;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Text;
using fluXis.Game.Input;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Overlay.Auth.UI;
using fluXis.Game.Overlay.Notifications;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Graphics;

namespace fluXis.Game.Overlay.Auth;

public partial class RegisterOverlay : Container, IKeyBindingHandler<FluXisGlobalKeybind>
{
    [Resolved]
    private NotificationManager notifications { get; set; }

    [Resolved]
    private IAPIClient api { get; set; }

    [Resolved]
    private UISamples samples { get; set; }

    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private FluXisGame game { get; set; }

    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private LoginOverlay loginOverlay { get; set; }

    private Container content;
    private FillFlowContainer warningContainer;
    private FillFlowContainer formContainer;
    private Container loadingLayer;

    private FluXisSpriteText errorText;
    private AuthOverlayTextBox username;
    private AuthOverlayTextBox password;
    private AuthOverlayTextBox email;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        Alpha = 0;

        InternalChildren = new Drawable[]
        {
            new FullInputBlockingContainer
            {
                RelativeSizeAxes = Axes.Both,
                OnClickAction = Hide,
                Child = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color4.Black,
                    Alpha = .5f
                }
            },
            content = new ClickableContainer
            {
                AutoSizeAxes = Axes.Both,
                AutoSizeDuration = 400,
                AutoSizeEasing = Easing.OutQuint,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                CornerRadius = 20,
                Masking = true,
                EdgeEffect = FluXisStyles.ShadowMedium,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = FluXisColors.Background2
                    },
                    warningContainer = new FillFlowContainer
                    {
                        Width = 580,
                        AutoSizeAxes = Axes.Y,
                        Direction = FillDirection.Vertical,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Padding = new MarginPadding(40),
                        Spacing = new Vector2(10),
                        Children = new[]
                        {
                            createWarning(),
                            Empty().With(d => d.Anchor = d.Origin = Anchor.TopCentre),
                            new AuthOverlayButton("Yes, I understand.") { Action = showForm }
                        }
                    },
                    formContainer = new FillFlowContainer
                    {
                        Width = 480,
                        AutoSizeAxes = Axes.Y,
                        Direction = FillDirection.Vertical,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Padding = new MarginPadding(40),
                        Spacing = new Vector2(10),
                        Alpha = 0,
                        Children = new[]
                        {
                            new FluXisSpriteText
                            {
                                Text = "Register",
                                Anchor = Anchor.TopCentre,
                                Origin = Anchor.TopCentre,
                                WebFontSize = 32
                            },
                            errorText = new TruncatingText
                            {
                                Text = "error message",
                                Anchor = Anchor.TopCentre,
                                Origin = Anchor.TopCentre,
                                MaxWidth = 300,
                                Colour = FluXisColors.Red,
                                WebFontSize = 14,
                                Alpha = 0
                            },
                            Empty().With(d => d.Anchor = d.Origin = Anchor.TopCentre),
                            username = new AuthOverlayTextBox
                            {
                                PlaceholderText = "Username",
                                TabbableContentContainer = this
                            },
                            email = new AuthOverlayTextBox
                            {
                                PlaceholderText = "Email",
                                TabbableContentContainer = this
                            },
                            password = new AuthOverlayTextBox
                            {
                                PlaceholderText = "Password",
                                TabbableContentContainer = this,
                                IsPassword = true
                            },
                            Empty().With(d => d.Anchor = d.Origin = Anchor.TopCentre),
                            new AuthOverlayButton("Create!") { Action = register },
                            new AuthOverlayButton("Back") { Action = openLogin }
                        }
                    },
                    loadingLayer = new FullInputBlockingContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        Alpha = 0,
                        Children = new Drawable[]
                        {
                            new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Colour = Colour4.Black,
                                Alpha = .5f
                            },
                            new LoadingIcon
                            {
                                Size = new Vector2(50),
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre
                            }
                        }
                    }
                }
            }
        };

        api.Status.BindValueChanged(onStatusChanged);
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        username.OnCommit += (_, _) => switchFocus(email);
        email.OnCommit += (_, _) => switchFocus(password);
        password.OnCommit += (_, _) => register();
    }

    private FluXisTextFlow createWarning()
    {
        var flow = new FluXisTextFlow
        {
            WebFontSize = 14,
            Anchor = Anchor.TopCentre,
            Origin = Anchor.TopCentre,
            TextAnchor = Anchor.Centre,
            AutoSizeAxes = Axes.Y,
            Width = 500
        };

        flow.AddText("fluXis allows only");
        flow.AddText(" one account per person.", t => t.Colour = FluXisColors.Red);
        flow.NewParagraph();
        flow.AddText("please make sure you check that you are not already registered.");
        flow.NewParagraph();
        flow.AddText("creating multiple accounts will result in ");
        flow.AddText("a ban of both accounts.", t => t.Colour = FluXisColors.Red);
        flow.NewParagraph();
        flow.NewParagraph(); // love spamming these
        flow.NewParagraph();
        flow.AddText("by registering, you also agree to the ");
        flow.NewParagraph();
        flow.AddText<ClickableFluXisSpriteText>("Terms of Service", t =>
        {
            t.Colour = FluXisColors.Link;
            t.Action = () => game?.OpenLink("https://fluxis.flux.moe/wiki/legal/terms");
        });
        flow.AddText(" and ");
        flow.AddText<ClickableFluXisSpriteText>("Privacy Policy", t =>
        {
            t.Colour = FluXisColors.Link;
            t.Action = () => game?.OpenLink("https://fluxis.flux.moe/wiki/legal/privacy");
        });

        return flow;
    }

    private void showForm()
    {
        warningContainer.FadeOut(200);
        formContainer.Delay(100).FadeIn(200);
        switchFocus(username);
    }

    private void openLogin()
    {
        Hide();
        loginOverlay?.Show();
    }

    private void onStatusChanged(ValueChangedEvent<ConnectionStatus> e)
    {
        Schedule(() =>
        {
            if (e.NewValue == ConnectionStatus.Failing)
                setError(api.LastException?.Message ?? "Failed to connect to the server.");

            switch (e.NewValue)
            {
                case ConnectionStatus.Online:
                    Hide();
                    break;

                case ConnectionStatus.Offline:
                case ConnectionStatus.Failing:
                    loadingLayer.FadeOut(200);
                    break;

                default:
                    loadingLayer.FadeIn(200);
                    break;
            }
        });
    }

    private void switchFocus(AuthOverlayTextBox to) => Schedule(() => GetContainingFocusManager()?.ChangeFocus(to));

    private void setError(string msg)
    {
        if (string.IsNullOrEmpty(msg))
        {
            errorText.Alpha = 0;
            return;
        }

        errorText.Text = msg;
        errorText.Alpha = 1;
    }

    private void register()
    {
        if (api.Status.Value == ConnectionStatus.Online)
        {
            notifications.SendError("Already logged in!");
            return;
        }

        if (string.IsNullOrEmpty(username.Text))
        {
            setError("Username cannot be empty.");
            return;
        }

        if (string.IsNullOrEmpty(email.Text))
        {
            setError("Email cannot be empty.");
            return;
        }

        if (string.IsNullOrEmpty(password.Text) || password.Text.Length < 8 || password.Text.Length > 32)
        {
            setError("Password must be between 8 and 32 characters.");
            return;
        }

        api.Register(username.Text, password.Text, email.Text);
    }

    public override void Show()
    {
        warningContainer.Show();
        formContainer.Hide();

        this.FadeInFromZero(400, Easing.OutQuint);
        content.ScaleTo(.75f).ScaleTo(1f, 800, Easing.OutElasticHalf);
        samples.Overlay(false);
    }

    public override void Hide()
    {
        this.FadeOut(400, Easing.OutQuint);
        content.ScaleTo(.9f, 400, Easing.OutQuint);
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

using fluXis.Audio;
using fluXis.Configuration;
using fluXis.Graphics;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Online.Fluxel;
using fluXis.Overlay.Auth.UI;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;
using osuTK.Graphics;

namespace fluXis.Overlay.Auth;

public partial class LoginOverlay : CompositeDrawable
{
    [Resolved]
    private IAPIClient api { get; set; }

    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private FluXisGame game { get; set; }

    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private RegisterOverlay registerOverlay { get; set; }

    [Resolved]
    private UISamples samples { get; set; }

    private Container content;
    private FluXisSpriteText errorText;
    private AuthOverlayTextBox username;
    private AuthOverlayTextBox password;
    private Container loadingLayer;

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config)
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
                    new FillFlowContainer
                    {
                        Width = 380,
                        AutoSizeAxes = Axes.Y,
                        Direction = FillDirection.Vertical,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Padding = new MarginPadding(40),
                        Spacing = new Vector2(10),
                        Children = new[]
                        {
                            new FluXisSpriteText
                            {
                                Text = "Login",
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
                                TabbableContentContainer = this,
                                Text = config.Get<string>(FluXisSetting.Username),
                                PlaceholderText = "Username"
                            },
                            password = new AuthOverlayTextBox
                            {
                                TabbableContentContainer = this,
                                PlaceholderText = "Password",
                                IsPassword = true
                            },
                            Empty().With(d => d.Anchor = d.Origin = Anchor.TopCentre),
                            new AuthOverlayButton("Continue") { Action = login },
                            new AuthOverlayButton("Forgot password?") { Action = openPasswordReset },
                            new AuthOverlayButton("Create new account") { Action = openRegister },
                            new AuthOverlayButton("Play offline")
                            {
                                Action = Hide,
                                Color = FluXisColors.Background3,
                                TextColor = FluXisColors.Text,
                            }
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
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        password.OnCommit += (_, _) => login();
        api.Status.BindValueChanged(updateStatus, true);
    }

    private void updateStatus(ValueChangedEvent<ConnectionStatus> e)
    {
        Schedule(() =>
        {
            if (e.NewValue == ConnectionStatus.Failed)
                setError(api.LastException?.Message ?? "Failed to connect to the server.");

            switch (e.NewValue)
            {
                case ConnectionStatus.Online:
                    Hide();
                    break;

                case ConnectionStatus.Offline:
                case ConnectionStatus.Failed:
                    loadingLayer.FadeOut(200);
                    break;

                default:
                    loadingLayer.FadeIn(200);
                    break;
            }
        });
    }

    private void login()
    {
        setError("");

        if (string.IsNullOrEmpty(username.Text))
        {
            setError("Username cannot be empty.");
            return;
        }

        if (string.IsNullOrEmpty(password.Text))
        {
            setError("Password cannot be empty.");
            return;
        }

        api.Login(username.Text, password.Text);
    }

    private void openPasswordReset() => game?.OpenLink("https://auth.flux.moe/request-reset");

    private void openRegister()
    {
        Hide();
        registerOverlay?.Show();
    }

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

    public override void Show()
    {
        if (api.Status.Value == ConnectionStatus.Online)
            return;

        this.FadeInFromZero(400, Easing.OutQuint);
        content.ScaleTo(.75f).ScaleTo(1f, 800, Easing.OutElasticHalf);
        samples.Overlay(false);

        Schedule(() => GetContainingFocusManager()?.ChangeFocus(string.IsNullOrEmpty(username.Text) ? username : password));
    }

    public override void Hide()
    {
        this.FadeOut(400, Easing.OutQuint);
        content.ScaleTo(.9f, 400, Easing.OutQuint);
        samples.Overlay(true);
    }
}

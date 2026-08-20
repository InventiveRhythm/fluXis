using System;
using fluXis.Audio;
using fluXis.Configuration;
using fluXis.Graphics;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Panel;
using fluXis.Graphics.UserInterface.Panel.Types;
using fluXis.Integration;
using fluXis.Online.Fluxel;
using fluXis.Overlay.Auth.UI;
using fluXis.Utils.Extensions;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;

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

    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private ISteamManager steam { get; set; }

    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private PanelContainer panels { get; set; }

    [Resolved]
    private UISamples samples { get; set; }

    private Container content;
    private FluXisSpriteText errorText;

    private AuthOverlayButton switchToNew;
    private AuthOverlayButton loginWithSteam;
    private AuthOverlayButton register;

    private AuthOverlayButton switchToLegacy;
    private AuthOverlayTextBox legacyUsername;
    private AuthOverlayTextBox legacyPassword;
    private AuthOverlayButton legacyContinue;
    private AuthOverlayButton legacyForgot;

    private Container loadingLayer;

    private Action loginAction;
    private Action closeAction;

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config)
    {
        RelativeSizeAxes = Axes.Both;
        Alpha = 0;

        InternalChildren = new Drawable[]
        {
            content = new ClickableContainer
            {
                AutoSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                CornerRadius = 20,
                Masking = true,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Theme.Background2
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
                                Colour = Theme.Red,
                                WebFontSize = 14,
                                Alpha = 0
                            },
                            Empty().With(d => d.Anchor = d.Origin = Anchor.TopCentre),
                            loginWithSteam = new AuthOverlayIconButton(FontAwesome.Brands.Steam, "Login with Steam")
                            {
                                Action = steamLogin,
                                Color = ColourInfo.GradientVertical(Colour4.FromHex("#1C2B43"), Colour4.FromHex("#106691")),
                                TextColor = Colour4.FromHex("#EDEDED")
                            },
                            legacyUsername = new AuthOverlayTextBox
                            {
                                TabbableContentContainer = this,
                                Text = config.Get<string>(FluXisSetting.Username),
                                PlaceholderText = "Username",
                                Alpha = 0
                            },
                            legacyPassword = new AuthOverlayTextBox
                            {
                                TabbableContentContainer = this,
                                PlaceholderText = "Password",
                                IsPassword = true,
                                Alpha = 0
                            },
                            Empty().With(d => d.Anchor = d.Origin = Anchor.TopCentre),
                            register = new AuthOverlayButton("Create new account") { Action = openRegister },
                            switchToLegacy = new AuthOverlayButton("Login with username/email")
                            {
                                Action = () => switchModes(true)
                            },
                            legacyContinue = new AuthOverlayButton("Continue") { Action = legacyLogin, Alpha = 0 },
                            legacyForgot = new AuthOverlayButton("Forgot password?") { Action = openPasswordReset, Alpha = 0 },
                            switchToNew = new AuthOverlayButton("Back")
                            {
                                Alpha = 0,
                                Action = () => switchModes()
                            },
                            new AuthOverlayButton("Play offline")
                            {
                                Action = () =>
                                {
                                    closeAction?.Invoke();
                                    Hide();
                                },
                                Color = Theme.Background3,
                                TextColor = Theme.Text,
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

        legacyPassword.OnCommit += (_, _) => legacyLogin();
    }

    private void switchModes(bool legacy = false)
    {
        errorText.Alpha = 0;

        loginWithSteam.Alpha = switchToLegacy.Alpha = register.Alpha = legacy ? 0f : 1f;
        legacyUsername.Alpha = legacyPassword.Alpha = legacyContinue.Alpha = legacyForgot.Alpha = switchToNew.Alpha = legacy ? 1f : 0f;
    }

    private async void steamLogin()
    {
        try
        {
            Scheduler.ScheduleIfNeeded(() => loadingLayer.FadeIn(Styling.TRANSITION_FADE));

            var ticket = "invalid-ticket";
            if (steam != null) ticket = await steam.GetAuthTicket();
            var error = await api.LoginSteam(ticket);

            Scheduler.ScheduleIfNeeded(() => loadingLayer.FadeOut(Styling.TRANSITION_FADE));

            if (error != null)
            {
                panels?.Add(new SingleButtonPanel(Phosphor.Bold.Warning, "Failed to login with steam!", error.Message));
                if (panels == null) setError(error.Message);
                return;
            }

            loginAction?.Invoke();
            Hide();
        }
        catch (Exception e)
        {
            setError($"{e}: {e.Message}");
        }
    }

    private async void legacyLogin()
    {
        try
        {
            setError("");

            if (string.IsNullOrEmpty(legacyUsername.Text))
            {
                setError("Username cannot be empty.");
                return;
            }

            if (string.IsNullOrEmpty(legacyPassword.Text))
            {
                setError("Password cannot be empty.");
                return;
            }

            var error = await api.LoginLegacy(legacyUsername.Text, legacyPassword.Text);

            if (error != null)
            {
                setError(error.Message);
                return;
            }

            loginAction?.Invoke();
            Hide();
        }
        catch (Exception e)
        {
            setError($"{e}: {e.Message}");
        }
    }

    private void openPasswordReset() => game?.OpenLink("https://auth.flux.moe/request-reset");

    private void openRegister()
    {
        Hide();
        registerOverlay?.Show(loginAction);
    }

    private void setError(string msg) => Scheduler.ScheduleIfNeeded(() =>
    {
        if (string.IsNullOrEmpty(msg))
        {
            loadingLayer.FadeIn(200);
            errorText.Alpha = 0;
            return;
        }

        errorText.Text = msg;
        errorText.Alpha = 1;
        loadingLayer.FadeOut(200);
    });

    public void Show(Action login, Action close)
    {
        loginAction = login;
        closeAction = close;
        Show();
    }

    public override void Show()
    {
        this.FadeInFromZero(400, Easing.OutQuint);
        content.ScaleTo(.75f).ScaleTo(1f, 800, Easing.OutElasticHalf);
        samples.Overlay(false);

        Schedule(() => GetContainingFocusManager()?.ChangeFocus(string.IsNullOrEmpty(legacyUsername.Text) ? legacyUsername : legacyPassword));
    }

    public override void Hide()
    {
        this.FadeOut(400, Easing.OutQuint);
        content.ScaleTo(.9f, 400, Easing.OutQuint);
        samples.Overlay(true);
    }
}

using fluXis.Game.Configuration;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface;
using fluXis.Game.Graphics.UserInterface.Buttons;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Text;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Overlay.Register;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Framework.Logging;
using osuTK;
using osuTK.Graphics;

namespace fluXis.Game.Overlay.Login;

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

    private Container content;
    private FluXisSpriteText errorText;
    private TextBox username;
    private TextBox password;
    private Container loadingLayer;

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
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                WebFontSize = 32
                            },
                            errorText = new TruncatingText
                            {
                                Text = "error message",
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                MaxWidth = 300,
                                Colour = FluXisColors.Red,
                                WebFontSize = 14,
                                Alpha = 0
                            },
                            Empty().With(d => d.Anchor = d.Origin = Anchor.Centre),
                            username = new TextBox
                            {
                                TabbableContentContainer = this,
                                Text = config.Get<string>(FluXisSetting.Username),
                                PlaceholderText = "Username"
                            },
                            password = new TextBox
                            {
                                TabbableContentContainer = this,
                                PlaceholderText = "Password",
                                IsPassword = true
                            },
                            Empty().With(d => d.Anchor = d.Origin = Anchor.Centre),
                            new Button("Continue") { Action = login },
                            new Button("Forgot password?") { Action = openPasswordReset },
                            new Button("Create new account") { Action = openRegister }
                        }
                    },
                    loadingLayer = new Container
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
        {
            Logger.Log(api.GetType().FullName);
            Logger.Log("Already connected, skipping login overlay.", LoggingTarget.Network, LogLevel.Debug);
            return;
        }

        this.FadeInFromZero(400, Easing.OutQuint);
        content.ScaleTo(.75f).ScaleTo(1f, 800, Easing.OutElasticHalf);

        Schedule(() => GetContainingInputManager().ChangeFocus(string.IsNullOrEmpty(username.Text) ? username : password));
    }

    public override void Hide()
    {
        this.FadeOut(400, Easing.OutQuint);
        content.ScaleTo(.9f, 400, Easing.OutQuint);
    }

    private partial class Button : FluXisButton
    {
        public Button(string text)
        {
            Text = text;
            RelativeSizeAxes = Axes.X;
            Height = 40;
            Color = FluXisColors.Highlight;
            TextColor = FluXisColors.Background2;
            FontSize = FluXisSpriteText.GetWebFontSize(14);
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
        }
    }

    private partial class TextBox : FluXisTextBox
    {
        public TextBox()
        {
            RelativeSizeAxes = Axes.X;
            Height = 50;
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            CornerRadius = 25;
            FontSize = FluXisSpriteText.GetWebFontSize(14);
            SidePadding = 20;
            BorderThickness = 2;
            BorderColour = FluXisColors.Background4;
        }

        protected override void OnFocus(FocusEvent e)
        {
            base.OnFocus(e);
            this.TransformTo(nameof(BorderColour), (ColourInfo)FluXisColors.Highlight, 200);
        }

        protected override void OnFocusLost(FocusLostEvent e)
        {
            base.OnFocusLost(e);
            this.TransformTo(nameof(BorderColour), (ColourInfo)FluXisColors.Background4, 200);
        }
    }
}

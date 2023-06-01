using System;
using fluXis.Game.Configuration;
using fluXis.Game.Graphics;
using fluXis.Game.Online.API;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Online.Fluxel.Packets.Account;
using fluXis.Game.Overlay.Login.UI;
using fluXis.Game.Overlay.Notification;
using fluXis.Game.Overlay.Register;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;
using osuTK.Graphics;

namespace fluXis.Game.Overlay.Login;

public partial class LoginOverlay : Container
{
    [Resolved]
    private NotificationOverlay notifications { get; set; }

    [Resolved]
    private RegisterOverlay register { get; set; }

    private Bindable<string> tokenBind;

    private LoginContent loginContainer;
    private SpriteText loadingText;

    private LoginTextBox username;
    private LoginTextBox password;

    private ClickableContainer content;

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
                    Alpha = 0.5f
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
                    loadingText = new SpriteText
                    {
                        Text = "",
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Font = FluXisFont.Default(24)
                    }
                }
            }
        };

        Fluxel.RegisterListener<string>(EventType.Token, onAuth);
        Fluxel.RegisterListener<APIUserShort>(EventType.Login, onLogin);

        tokenBind = config.GetBindable<string>(FluXisSetting.Token);
        if (!string.IsNullOrEmpty(tokenBind?.Value)) sendLogin();
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

        if (Fluxel.IsConnected)
        {
            loadingText.Text = "Logging in...";
            loadingText.FadeIn(200).OnComplete(_ => Fluxel.SendPacket(new AuthPacket(username.Text, password.Text)));
        }
        else
        {
            setLoadingText("Not connected to any server!", switchToLogin);
        }
    }

    private void onAuth(FluxelResponse<string> response)
    {
        if (response.Status == 200)
        {
            setLoadingText("Getting user data...", () => Fluxel.SendPacket(new LoginPacket(response.Data)));
            Fluxel.Token = response.Data;

            if (tokenBind != null)
                tokenBind.Value = response.Data;
        }
        else setLoadingText(response.Message, switchToLogin);
    }

    private void sendLogin()
    {
        switchToLoading();
        loginContainer.Alpha = 0;
        Fluxel.Token = tokenBind.Value;
        setLoadingText("Logging in...", () => Fluxel.SendPacket(new LoginPacket(tokenBind.Value)));
    }

    private void onLogin(FluxelResponse<APIUserShort> response)
    {
        if (response.Status == 200)
        {
            Fluxel.LoggedInUser = response.Data;
            setLoadingText("Logged in!", Hide);
        }
        else setLoadingText(response.Message, switchToLogin);
    }

    private void onRegister(FluxelResponse<APIRegisterResponse> response)
    {
        if (response.Status == 200)
        {
            Fluxel.LoggedInUser = response.Data.User;
            Fluxel.Token = response.Data.Token;
            tokenBind.Value = response.Data.Token;
            setLoadingText("Registered!", Hide);
        }
        else setLoadingText(response.Message, () => { });
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
        this.FadeIn(200);
        content.ResizeHeightTo(160, 400, Easing.OutQuint);
    }

    public override void Hide()
    {
        this.FadeOut(200);
        content.ResizeHeightTo(0, 200, Easing.InQuint);
    }

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

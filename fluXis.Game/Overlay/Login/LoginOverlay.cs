using System;
using fluXis.Game.Configuration;
using fluXis.Game.Graphics;
using fluXis.Game.Online.API;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Online.Fluxel.Packets.Account;
using fluXis.Game.Overlay.Login.UI;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Game.Overlay.Login;

public partial class LoginOverlay : Container
{
    private Bindable<string> tokenBind;

    public bool LoggedIn { get; set; }

    private readonly LoginContent loginContainer;
    private readonly LoginContent registerContainer;
    private readonly SpriteText loadingText;

    private readonly LoginTextBox usernameLoginTextBox;
    private readonly LoginTextBox passwordLoginTextBox;

    private readonly LoginTextBox usernameRegisterTextBox;
    private readonly LoginTextBox passwordRegisterTextBox;
    private readonly LoginTextBox confirmPasswordRegisterTextBox;

    public LoginOverlay()
    {
        Anchor = Origin = Anchor.Centre;
        Width = 300;
        Height = 150;
        CornerRadius = 10;
        Masking = true;

        AddRange(new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background2
            },
            loginContainer = new LoginContent
            {
                Children = new Drawable[]
                {
                    usernameLoginTextBox = new LoginTextBox(false, "Username"),
                    passwordLoginTextBox = new LoginTextBox(true, "Password"),
                    new LoginButton("Create new...")
                    {
                        Action = switchToRegister
                    },
                    new LoginButton("Login!")
                    {
                        Action = login
                    }
                }
            },
            registerContainer = new LoginContent
            {
                Alpha = 0,
                Children = new Drawable[]
                {
                    usernameRegisterTextBox = new LoginTextBox(false, "Username"),
                    passwordRegisterTextBox = new LoginTextBox(true, "Password"),
                    confirmPasswordRegisterTextBox = new LoginTextBox(true, "Confirm Password"),
                    new LoginButton("Back to Login...")
                    {
                        Action = switchToLogin
                    },
                    new LoginButton("Create!")
                    {
                        Action = register
                    }
                }
            },
            loadingText = new SpriteText
            {
                Text = "",
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Font = new FontUsage("Quicksand", 30, "SemiBold"),
                Alpha = 0
            }
        });

        Fluxel.RegisterListener<string>(EventType.Token, onAuth);
        Fluxel.RegisterListener<APIUser>(EventType.Login, onLogin);
        Fluxel.RegisterListener<APIRegisterResponse>(EventType.Register, onRegister);
    }

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config)
    {
        tokenBind = config.GetBindable<string>(FluXisSetting.Token);

        if (!string.IsNullOrEmpty(tokenBind?.Value)) sendLogin();
    }

    private void switchToRegister(LoginButton button)
    {
        loadingText.FadeOut(200);
        loginContainer.FadeOut(200)
                      .OnComplete(_ =>
                      {
                          this.ResizeHeightTo(190, 200, Easing.OutQuint)
                              .ResizeWidthTo(300, 200, Easing.OutQuint)
                              .OnComplete(_ => registerContainer.FadeIn(200));
                      });
    }

    private void switchToLogin(LoginButton button)
    {
        loadingText.FadeOut(200);
        registerContainer.FadeOut(200)
                         .OnComplete(_ =>
                         {
                             this.ResizeHeightTo(150, 200, Easing.OutQuint)
                                 .ResizeWidthTo(300, 200, Easing.OutQuint)
                                 .OnComplete(_ => loginContainer.FadeIn(200));
                         });
    }

    private void switchToLoading()
    {
        loginContainer.FadeOut(200);
        registerContainer.FadeOut(200).OnComplete(_ =>
        {
            this.ResizeHeightTo(100, 200, Easing.OutQuint)
                .ResizeWidthTo(450, 200, Easing.OutQuint);
        });
    }

    private void login(LoginButton button)
    {
        switchToLoading();
        loadingText.Text = "Logging in...";
        loadingText.FadeIn(200).OnComplete(_ => Fluxel.SendPacket(new AuthPacket(usernameLoginTextBox.Text, passwordLoginTextBox.Text)));
    }

    private void onAuth(FluxelResponse<string> response)
    {
        if (response.Status == 200)
        {
            setLoadingText("Getting user data...", () => Fluxel.SendPacket(new LoginPacket(response.Data)));
            Fluxel.Token = response.Data;
            tokenBind.Value = response.Data;
        }
        else setLoadingText(response.Message, () => switchToLogin(null));
    }

    private void sendLogin()
    {
        switchToLoading();
        loginContainer.Alpha = 0;
        setLoadingText("Logging in...", () => Fluxel.SendPacket(new LoginPacket(tokenBind.Value)));
    }

    private void onLogin(FluxelResponse<APIUser> response)
    {
        if (response.Status == 200)
        {
            LoggedIn = true;
            Fluxel.SetLoggedInUser(response.Data);
            setLoadingText("Logged in!", () => this.FadeOut(200));
        }
        else setLoadingText(response.Message, () => switchToLogin(null));
    }

    private void register(LoginButton button)
    {
        switchToLoading();
        loadingText.Text = "Registering...";
        loadingText.FadeIn(200).OnComplete(_ =>
        {
            if (passwordRegisterTextBox.Text != confirmPasswordRegisterTextBox.Text)
            {
                setLoadingText("Passwords don't match!", () => switchToRegister(null));
                return;
            }

            Fluxel.SendPacket(new RegisterPacket(usernameRegisterTextBox.Text, passwordRegisterTextBox.Text));
        });
    }

    private void onRegister(FluxelResponse<APIRegisterResponse> response)
    {
        if (response.Status == 200)
        {
            LoggedIn = true;
            Fluxel.SetLoggedInUser(response.Data.User);
            Fluxel.Token = response.Data.Token;
            tokenBind.Value = response.Data.Token;
            setLoadingText("Registered!", () => this.FadeOut(200));
        }
        else setLoadingText(response.Message, () => switchToRegister(null));
    }

    private void setLoadingText(string text, Action onComplete)
    {
        loadingText.FadeOut(200)
                   .OnComplete(_ =>
                   {
                       loadingText.Text = text;
                       loadingText.FadeIn(200)
                                  .Then(400).FadeIn()
                                  .OnComplete(_ => onComplete?.Invoke());
                   });
    }

    private partial class LoginContent : FillFlowContainer
    {
        public LoginContent()
        {
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;
            Padding = new MarginPadding(20);
            Spacing = new Vector2(10, 10);
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
        }
    }
}

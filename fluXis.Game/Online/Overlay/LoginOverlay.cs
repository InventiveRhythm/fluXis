using fluXis.Game.Configuration;
using fluXis.Game.Graphics;
using fluXis.Game.Integration;
using fluXis.Game.Online.API;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Online.Fluxel.Packets.Account;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osuTK;

namespace fluXis.Game.Online.Overlay
{
    public partial class LoginOverlay : Container
    {
        private Bindable<string> tokenBind;

        private readonly OnlineOverlay onlineOverlay;

        private TextBox username;
        private PasswordTextBox password;
        private BasicButton loginButton;

        public LoginOverlay(OnlineOverlay onlineOverlay)
        {
            this.onlineOverlay = onlineOverlay;
        }

        [BackgroundDependencyLoader]
        private void load(FluXisConfig config)
        {
            tokenBind = config.GetBindable<string>(FluXisSetting.Token);

            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            Width = 300;
            Height = 190;
            Masking = true;
            CornerRadius = 10;

            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Colour4.FromHex("#1a1a20")
                },
                new SpriteText
                {
                    Text = "Login",
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Position = new Vector2(0, 10),
                    Font = new FontUsage("Quicksand", 30, "SemiBold")
                },
                username = new TextBox
                {
                    Width = 200,
                    Height = 30,
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Position = new Vector2(0, 50),
                },
                password = new PasswordTextBox
                {
                    Width = 200,
                    Height = 30,
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Position = new Vector2(0, 90),
                },
                loginButton = new BasicButton
                {
                    Width = 200,
                    Height = 30,
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Position = new Vector2(0, 130),
                    Text = "Submit",
                    Enabled = { Value = true },
                    Action = submit
                }
            };

            Alpha = 0;
            Scale = new Vector2(0.9f);

            Fluxel.Fluxel.RegisterListener<string>(EventType.Token, onToken);
            Fluxel.Fluxel.RegisterListener<APIUser>(EventType.Login, onLogin);

            if (string.IsNullOrEmpty(tokenBind.Value)) return;

            loginButton.Enabled.Value = false;
            Fluxel.Fluxel.SendPacket(new LoginPacket(tokenBind.Value));
            Fluxel.Fluxel.Token = tokenBind.Value;
        }

        private void submit()
        {
            loginButton.Enabled.Value = false;
            Fluxel.Fluxel.SendPacket(new AuthPacket(username.Text, password.Text));
        }

        private void onToken(FluxelResponse<string> packet)
        {
            if (packet.Status == 200)
            {
                Fluxel.Fluxel.SendPacket(new LoginPacket(packet.Data));
                Fluxel.Fluxel.Token = packet.Data;
                tokenBind.Value = packet.Data;
            }
            else
                loginButton.Enabled.Value = true;
        }

        private void onLogin(FluxelResponse<APIUser> packet)
        {
            if (packet.Status == 200)
            {
                Fluxel.Fluxel.SetLoggedInUser(packet.Data);
                onlineOverlay.OnUserLogin();
                completed = true;
                Discord.Reload();
            }
            else
                loginButton.Enabled.Value = true;
        }

        private bool completed;
        private bool visible = true;

        protected override void Update()
        {
            if (completed && visible)
            {
                Hide();
                visible = false;
            }

            if (completed && !visible && Alpha == 0)
            {
                Dispose();
                completed = false;
            }

            base.Update();
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);
            Fluxel.Fluxel.UnregisterListener(0);
        }

        public override void Hide()
        {
            this.FadeOut(200).ScaleTo(.9f, 200, Easing.InQuint);
        }

        public override void Show()
        {
            if (!visible)
                return;

            this.FadeIn(200).ScaleTo(1f, 200, Easing.OutElastic);
        }

        private partial class TextBox : BasicTextBox
        {
            public TextBox()
            {
                BackgroundUnfocused = Colour4.FromHex("#222228");
                BackgroundFocused = Colour4.FromHex("#7e7e7f");
                CornerRadius = 5;
                Masking = true;
                // SelectionColour = Colour4.FromHex("#7e7e7f");
            }
        }

        private partial class PasswordTextBox : TextBox
        {
            protected override Drawable GetDrawableCharacter(char c) => new FallingDownContainer()
            {
                Height = CalculatedTextSize,
                Width = CalculatedTextSize / 2,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Child = new PasswordChar(CalculatedTextSize)
            };

            private partial class PasswordChar : TicTac
            {
                public PasswordChar(float size)
                    : base(size)
                {
                    Anchor = Anchor.Centre;
                    Origin = Anchor.Centre;
                    Size = new Vector2(size / 2);
                }

                protected override void LoadComplete()
                {
                    base.LoadComplete();
                    this.ScaleTo(0).ScaleTo(1, 100);
                }
            }
        }
    }
}

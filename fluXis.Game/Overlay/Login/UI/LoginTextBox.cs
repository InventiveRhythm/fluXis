using fluXis.Game.Graphics;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osuTK;
using osuTK.Graphics;

namespace fluXis.Game.Overlay.Login.UI;

public partial class LoginTextBox : FluXisTextBox
{
    protected override Color4 SelectionColour => FluXisColors.Accent2;

    private readonly bool isPassword;

    public LoginTextBox(bool password, string placeholder)
    {
        isPassword = password;

        Height = 30;
        Width = 260;
        Anchor = Anchor.TopCentre;
        Origin = Anchor.TopCentre;
        BackgroundUnfocused = FluXisColors.Surface;
        BackgroundFocused = FluXisColors.Surface2;
        CornerRadius = 5;
        Masking = true;

        PlaceholderText = placeholder;
        Placeholder.Font = FluXisFont.Default();
        Placeholder.Colour = FluXisColors.Accent3;
        Placeholder.Anchor = Anchor.CentreLeft;
        Placeholder.Origin = Anchor.CentreLeft;
    }

    protected override Drawable GetDrawableCharacter(char c)
    {
        var container = new FallingDownContainer
        {
            Anchor = Anchor.CentreLeft,
            Origin = Anchor.CentreLeft,
        };

        if (isPassword)
        {
            container.Height = CalculatedTextSize;
            container.Width = CalculatedTextSize / 2;
            container.Child = new PasswordChar(CalculatedTextSize);
        }
        else
        {
            container.AutoSizeAxes = Axes.Both;
            container.Child = new SpriteText
            {
                Text = c.ToString(),
                Font = FluXisFont.Default(CalculatedTextSize),
            };
        }

        return container;
    }

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

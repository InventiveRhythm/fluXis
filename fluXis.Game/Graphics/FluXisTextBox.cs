using System;
using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Utils;
using osuTK;
using osuTK.Graphics;

namespace fluXis.Game.Graphics;

public partial class FluXisTextBox : BasicTextBox
{
    protected override Color4 SelectionColour => FluXisColors.Accent2;

    public bool IsPassword { get; set; }
    public Action OnTextChanged;

    private List<Sample> textAdded;
    private Sample accept;
    private Sample delete;
    private Sample error;

    [BackgroundDependencyLoader]
    private void load(ISampleStore samples)
    {
        CornerRadius = 5;
        Masking = true;
        BackgroundUnfocused = FluXisColors.Surface;
        BackgroundFocused = FluXisColors.Hover;
        BackgroundCommit = FluXisColors.Click;
        Placeholder.Font = FluXisFont.Default();
        Placeholder.Colour = FluXisColors.Text2;
        Placeholder.Anchor = Anchor.CentreLeft;
        Placeholder.Origin = Anchor.CentreLeft;

        textAdded = new List<Sample>(3);

        for (int i = 0; i < textAdded.Capacity; i++)
            textAdded.Add(samples.Get($@"UI/Keyboard/tap-{i + 1}"));

        accept = samples.Get(@"UI/Keyboard/confirm");
        delete = samples.Get(@"UI/Keyboard/delete");
        error = samples.Get(@"UI/Keyboard/error");
    }

    protected override void OnUserTextAdded(string added)
    {
        textAdded[RNG.Next(textAdded.Count)]?.Play();
        OnTextChanged?.Invoke();
        base.OnUserTextAdded(added);
    }

    protected override void OnTextCommitted(bool textChanged)
    {
        accept?.Play();
        base.OnTextCommitted(textChanged);
    }

    public void NotifyError() => NotifyInputError();

    protected override void NotifyInputError()
    {
        error?.Play();
        base.NotifyInputError();
    }

    protected override Drawable GetDrawableCharacter(char c)
    {
        var container = new FallingDownContainer
        {
            Anchor = Anchor.CentreLeft,
            Origin = Anchor.CentreLeft
        };

        if (IsPassword)
        {
            container.Height = CalculatedTextSize;
            container.Width = CalculatedTextSize / 2;
            container.Child = new PasswordCharacter(CalculatedTextSize);
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

    protected override void OnUserTextRemoved(string removed)
    {
        base.OnUserTextRemoved(removed);
        delete?.Play();
        OnTextChanged?.Invoke();
    }

    private partial class PasswordCharacter : TicTac
    {
        public PasswordCharacter(float size)
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

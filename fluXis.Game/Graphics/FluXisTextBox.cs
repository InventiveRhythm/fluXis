using System;
using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Utils;
using osuTK.Graphics;

namespace fluXis.Game.Graphics;

public partial class FluXisTextBox : BasicTextBox
{
    protected override Color4 SelectionColour => FluXisColors.Accent2;

    public Action OnTextChanged;

    private List<Sample> textAdded;
    private Sample accept;
    private Sample error;

    [BackgroundDependencyLoader]
    private void load(ISampleStore samples)
    {
        CornerRadius = 5;
        Masking = true;
        BackgroundUnfocused = FluXisColors.Surface;
        BackgroundFocused = FluXisColors.Hover;
        BackgroundCommit = FluXisColors.Click;

        textAdded = new List<Sample>(3);

        for (int i = 0; i < textAdded.Capacity; i++)
            textAdded.Add(samples.Get($@"UI/Keyboard/tap-{i + 1}"));

        accept = samples.Get(@"UI/Keyboard/confirm");
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

    protected override Drawable GetDrawableCharacter(char c) => new FallingDownContainer
    {
        AutoSizeAxes = Axes.Both,
        Child = new SpriteText
        {
            Text = c.ToString(),
            Font = FluXisFont.Default(CalculatedTextSize)
        }
    };

    protected override void OnUserTextRemoved(string removed)
    {
        base.OnUserTextRemoved(removed);
        OnTextChanged?.Invoke();
    }
}

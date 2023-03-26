using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Utils;

namespace fluXis.Game.Graphics;

public partial class FluXisTextBox : BasicTextBox
{
    private List<Sample> textAdded;
    private Sample accept;
    private Sample error;

    [BackgroundDependencyLoader]
    private void load(ISampleStore samples)
    {
        textAdded = new List<Sample>(3);

        for (int i = 0; i < textAdded.Capacity; i++)
            textAdded.Add(samples.Get($@"UI/Keyboard/tap-{i + 1}"));

        accept = samples.Get(@"UI/Keyboard/confirm");
        error = samples.Get(@"UI/Keyboard/error");
    }

    protected override void OnUserTextAdded(string added)
    {
        textAdded[RNG.Next(textAdded.Count)]?.Play();
        base.OnUserTextAdded(added);
    }

    protected override void OnTextCommitted(bool textChanged)
    {
        accept?.Play();
        base.OnTextCommitted(textChanged);
    }

    protected override void NotifyInputError()
    {
        error?.Play();
        base.NotifyInputError();
    }
}

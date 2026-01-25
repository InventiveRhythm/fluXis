using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics.Containers;

namespace fluXis.Audio;

public partial class KeyboardSamples : CompositeDrawable
{
    private PitchVariatedSample tap;
    private PitchVariatedSample tapCaps;
    private Sample accept;
    private PitchVariatedSample delete;
    private Sample error;
    private DebouncedSample selectChar;
    private PitchVariatedSample selectWord;
    private PitchVariatedSample selectAll;

    [BackgroundDependencyLoader]
    private void load(ISampleStore samples)
    {
        AddInternal(tap = new PitchVariatedSample(samples.Get("UI/Keyboard/tap")));
        AddInternal(tapCaps = new PitchVariatedSample(samples.Get("UI/Keyboard/tap-caps")));

        accept = samples.Get("UI/Keyboard/confirm");
        AddInternal(delete = new PitchVariatedSample(samples.Get("UI/Keyboard/delete")));
        error = samples.Get("UI/Keyboard/error");

        AddInternal(selectChar = new DebouncedSample(new PitchVariatedSample(samples.Get("UI/Keyboard/select-char"))));
        AddInternal(selectWord = new PitchVariatedSample(samples.Get("UI/Keyboard/select-word")));
        AddInternal(selectAll = new PitchVariatedSample(samples.Get("UI/Keyboard/select-all")));
    }

    public void Tap(bool caps)
    {
        if (caps) tapCaps?.Play();
        else tap?.Play();
    }

    public void Accept() => accept?.Play();
    public void Delete() => delete?.Play();
    public void Error() => error?.Play();

    public void SelectChar() => selectChar.Play();
    public void SelectWord() => selectWord.Play();
    public void SelectAll() => selectAll.Play();
}

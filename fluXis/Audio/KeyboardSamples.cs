using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Utils;

namespace fluXis.Audio;

public partial class KeyboardSamples : Component
{
    private Sample tap;
    private Sample tapCaps;
    private Sample accept;
    private Sample delete;
    private Sample error;
    private Sample selectChar;
    private Sample selectWord;
    private Sample selectAll;

    private Bindable<double> tapPitch;
    private Bindable<double> selectPitch;

    private const float pitch_variation = 0.02f;
    private const int debounce_time = 50;

    private double lastSelectionTime;

    [BackgroundDependencyLoader]
    private void load(ISampleStore samples)
    {
        tapPitch = new Bindable<double>();
        selectPitch = new Bindable<double>();

        tap = samples.Get("UI/Keyboard/tap");
        tap?.Frequency.BindTo(tapPitch);

        tapCaps = samples.Get("UI/Keyboard/tap-caps");
        tapCaps?.Frequency.BindTo(tapPitch);

        accept = samples.Get("UI/Keyboard/confirm");
        delete = samples.Get("UI/Keyboard/delete");
        error = samples.Get("UI/Keyboard/error");

        selectChar = samples.Get("UI/Keyboard/select-char");
        selectChar?.Frequency.BindTo(selectPitch);

        selectWord = samples.Get("UI/Keyboard/select-word");
        selectWord?.Frequency.BindTo(selectPitch);

        selectAll = samples.Get("UI/Keyboard/select-all");
        selectAll?.Frequency.BindTo(selectPitch);
    }

    public void Tap(bool caps)
    {
        randomize(tapPitch);

        if (caps)
            tapCaps?.Play();
        else
            tap?.Play();
    }

    public void Accept() => accept?.Play();
    public void Delete() => delete?.Play();
    public void Error() => error?.Play();

    public void SelectChar()
    {
        if (Time.Current - lastSelectionTime < debounce_time)
            return;

        randomize(selectPitch);

        selectChar?.Play();
        lastSelectionTime = Time.Current;
    }

    public void SelectWord()
    {
        randomize(selectPitch);
        selectWord?.Play();
    }

    public void SelectAll()
    {
        randomize(selectPitch);
        selectAll?.Play();
    }

    private void randomize(Bindable<double> bind) => bind.Value = RNG.NextSingle(1 - pitch_variation, 1 + pitch_variation);
}

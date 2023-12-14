using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Utils;

namespace fluXis.Game.Audio;

public partial class UISamples : Component
{
    private Sample hover;
    private Sample click;
    private Sample dropdownOpen;
    private Sample dropdownClose;
    private Sample overlayOpen;
    private Sample overlayClose;

    private Bindable<double> hoverPitch;

    private const float pitch_variation = 0.02f;
    private const int debounce_time = 50;

    private double lastHoverTime;
    private double lastClickTime;

    [BackgroundDependencyLoader]
    private void load(ISampleStore samples)
    {
        hover = samples.Get("UI/hover");
        hover.Frequency.BindTo(hoverPitch = new Bindable<double>());

        click = samples.Get("UI/click");
        dropdownOpen = samples.Get("UI/dropdown-open");
        dropdownClose = samples.Get("UI/dropdown-close");
        overlayOpen = samples.Get("UI/Overlay/open");
        overlayClose = samples.Get("UI/Overlay/close");
    }

    public void Hover()
    {
        if (Time.Current - lastHoverTime < debounce_time)
            return;

        var rate = RNG.NextSingle(1 - pitch_variation, 1 + pitch_variation);
        hoverPitch.Value = rate;

        hover?.Play();
        lastHoverTime = Time.Current;
    }

    public void Click()
    {
        if (Time.Current - lastClickTime < debounce_time)
            return;

        click?.Play();
        lastClickTime = Time.Current;
    }

    public void Dropdown(bool close)
    {
        if (close)
            dropdownClose?.Play();
        else
            dropdownOpen?.Play();
    }

    public void Overlay(bool close)
    {
        if (close)
            overlayClose?.Play();
        else
            overlayOpen?.Play();
    }
}

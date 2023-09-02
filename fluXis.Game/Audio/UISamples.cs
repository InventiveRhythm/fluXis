using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;

namespace fluXis.Game.Audio;

public partial class UISamples : Component
{
    private Sample hover { get; set; }
    private Sample click { get; set; }

    private const int debounce_time = 50;

    private double lastHoverTime;
    private double lastClickTime;

    [BackgroundDependencyLoader]
    private void load(ISampleStore samples)
    {
        hover = samples.Get("UI/hover");
        click = samples.Get("UI/click");
    }

    public void Hover()
    {
        if (Time.Current - lastHoverTime < debounce_time)
            return;

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
}

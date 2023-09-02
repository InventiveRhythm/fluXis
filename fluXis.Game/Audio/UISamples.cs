using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;

namespace fluXis.Game.Audio;

public partial class UISamples : Component
{
    public Sample Hover { get; private set; }
    public Sample Click { get; private set; }

    [BackgroundDependencyLoader]
    private void load(ISampleStore samples)
    {
        Hover = samples.Get("UI/hover");
        Click = samples.Get("UI/click");
    }
}

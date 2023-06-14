using ManagedBass.Fx;
using osu.Framework.Audio.Mixing;
using osu.Framework.Graphics;

namespace fluXis.Game.Audio;

public partial class LowPassFilter : Component
{
    public AudioMixer Mixer { get; private set; }
    private readonly BQFParameters parameters;

    public const int MAX = 22049;
    public const int MIN = 1000;

    private int cutoff;

    public int Cutoff
    {
        get => cutoff;
        set
        {
            if (value >= MAX)
                removeFilter();
            else
                addFilter();

            if (value < MIN)
                value = MIN;

            cutoff = value;
            parameters.fCenter = cutoff;

            int filterIndex = Mixer.Effects.IndexOf(parameters);

            if (filterIndex < 0)
            {
                parameters.fCenter = value;
                Mixer.Effects.Add(parameters);
                return;
            }

            if (Mixer.Effects[filterIndex] is BQFParameters existingFilter)
            {
                existingFilter.fCenter = value;
                Mixer.Effects[filterIndex] = existingFilter;
            }
        }
    }

    public LowPassFilter(AudioMixer mixer)
    {
        Mixer = mixer;

        parameters = new BQFParameters
        {
            lFilter = BQFType.LowPass,
            fBandwidth = 0,
            fQ = .7f
        };

        Cutoff = MAX;
    }

    public void CutoffTo(int value, int duration = 0, Easing easing = Easing.None)
    {
        if (value >= MAX)
            removeFilter();
        else
            addFilter();

        if (value < MIN)
            value = MIN;

        this.TransformTo(nameof(Cutoff), value, duration, easing);
    }

    private void removeFilter()
    {
        if (Mixer.Effects.Contains(parameters))
            Mixer.Effects.Remove(parameters);
    }

    private void addFilter()
    {
        if (!Mixer.Effects.Contains(parameters))
            Mixer.Effects.Add(parameters);
    }
}

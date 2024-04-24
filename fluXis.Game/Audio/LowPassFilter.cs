using ManagedBass.Fx;
using osu.Framework.Audio.Mixing;
using osu.Framework.Graphics;

namespace fluXis.Game.Audio;

public partial class LowPassFilter : Component
{
    private AudioMixer mixer { get; }
    private readonly BQFParameters parameters;

    public bool Enabled => mixer.Effects.Contains(parameters);

    public const int MAX = 22049;
    public const int MIN = 1000;

    private int cutoff;

    public int Cutoff
    {
        get => cutoff;
        set
        {
            if (value == cutoff)
                return;

            if (value >= MAX)
            {
                removeFilter();
                return;
            }

            addFilter();

            if (value < MIN)
                value = MIN;

            cutoff = value;
            parameters.fCenter = cutoff;

            int filterIndex = mixer.Effects.IndexOf(parameters);

            if (filterIndex < 0)
            {
                parameters.fCenter = value;
                mixer.Effects.Add(parameters);
                return;
            }

            if (mixer.Effects[filterIndex] is BQFParameters existingFilter)
            {
                existingFilter.fCenter = value;
                mixer.Effects[filterIndex] = existingFilter;
            }
        }
    }

    public LowPassFilter(AudioMixer mixer)
    {
        this.mixer = mixer;

        parameters = new BQFParameters
        {
            lFilter = BQFType.LowPass,
            fBandwidth = 0,
            fQ = .7f
        };

        Cutoff = MAX;
    }

    public void CutoffTo(int value, float duration = 0, Easing easing = Easing.None)
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
        if (mixer.Effects.Contains(parameters))
            mixer.Effects.Remove(parameters);
    }

    private void addFilter()
    {
        if (!mixer.Effects.Contains(parameters))
            mixer.Effects.Add(parameters);
    }
}

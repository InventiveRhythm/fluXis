using System;
using ManagedBass.Fx;
using osu.Framework.Audio.Mixing;
using osu.Framework.Graphics;

namespace fluXis.Audio;

public partial class AudioFilter : Component
{
    private AudioMixer mixer { get; }
    private BQFType type { get; }
    private BQFParameters parameters { get; }

    private bool enabled { get; set; }

    public const int MAX = 22049;
    public const int MIN = 1000;

    private int cutoff = MAX;

    public int Cutoff
    {
        get => cutoff;
        set
        {
            if (value == cutoff)
                return;

            cutoff = value;

            switch (type)
            {
                case BQFType.LowPass:
                    if (value >= MAX)
                    {
                        removeFilter();
                        return;
                    }

                    value = Math.Max(MIN, value);
                    break;

                case BQFType.HighPass:
                    if (value <= 1)
                    {
                        removeFilter();
                        return;
                    }

                    break;
            }

            addFilter();

            parameters.fCenter = value;
            mixer.UpdateEffect(parameters);
        }
    }

    public AudioFilter(AudioMixer mixer, BQFType type = BQFType.LowPass)
    {
        this.mixer = mixer;
        this.type = type;

        parameters = new BQFParameters
        {
            lFilter = type,
            fBandwidth = 0,
            fQ = .7f
        };

        if (type == BQFType.HighPass)
            Cutoff = 1;
    }

    public void CutoffTo(int value, float duration = 0, Easing easing = Easing.None)
    {
        if (type == BQFType.LowPass)
            value = Math.Max(MIN, value);

        this.TransformTo(nameof(Cutoff), value, duration, easing);
    }

    private void removeFilter()
    {
        if (!enabled)
            return;

        mixer.RemoveEffect(parameters);
        enabled = false;
    }

    private void addFilter()
    {
        if (enabled)
            return;

        mixer.AddEffect(parameters);
        enabled = true;
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);
        removeFilter();
    }
}

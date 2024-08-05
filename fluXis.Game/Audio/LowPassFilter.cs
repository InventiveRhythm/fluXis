using System;
using ManagedBass.Fx;
using osu.Framework.Audio.Mixing;
using osu.Framework.Graphics;

namespace fluXis.Game.Audio;

public partial class LowPassFilter : Component
{
    private AudioMixer mixer { get; }
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

            if (value >= MAX)
            {
                removeFilter();
                return;
            }

            addFilter();

            value = Math.Max(MIN, value);
            parameters.fCenter = cutoff = value;
            mixer.UpdateEffect(parameters);
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
    }

    public void CutoffTo(int value, float duration = 0, Easing easing = Easing.None)
    {
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

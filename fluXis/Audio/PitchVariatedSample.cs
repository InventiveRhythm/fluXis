using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Audio;
using osu.Framework.Utils;

namespace fluXis.Audio;

public partial class PitchVariatedSample : DrawableAudioWrapper, ISample
{
    string ISample.Name => sample.Name;
    public double Length => sample.Length;
    public Bindable<int> PlaybackConcurrency => sample.PlaybackConcurrency;

    private ISample sample;
    private readonly float variation;

    public PitchVariatedSample(ISample sample, float variation = 0.04f)
        : base(sample)
    {
        this.sample = sample;
        this.variation = variation;
    }

    public void ReplaceSample(ISample s) => sample = s;

    public SampleChannel Play()
    {
        var channel = GetChannel();
        channel.Play();
        return channel;
    }

    public SampleChannel GetChannel()
    {
        var channel = sample.GetChannel();
        channel.Frequency.Value = 1f - variation / 2f + RNG.NextDouble(variation);
        return channel;
    }
}

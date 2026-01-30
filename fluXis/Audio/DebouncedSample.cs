using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Audio;

namespace fluXis.Audio;

#nullable enable

public partial class DebouncedSample : DrawableAudioWrapper, ISample
{
    string ISample.Name => sample.Name;
    public double Length => sample.Length;
    public Bindable<int> PlaybackConcurrency => sample.PlaybackConcurrency;

    public bool CanPlay => Time.Current - lastPlayed >= time;

    private ISample sample;
    private double time { get; }

    private double lastPlayed;

    public DebouncedSample(ISample sample, double time = 50)
        : base(sample)
    {
        this.sample = sample;
        this.time = time;
    }

    public void ReplaceSample(ISample s) => sample = s;
    public void UpdateLastPlayed() => lastPlayed = Time.Current;

    public SampleChannel Play()
    {
        var channel = GetChannel();

        if (!CanPlay)
            return channel;

        channel.Play();
        UpdateLastPlayed();
        return channel;
    }

    public SampleChannel GetChannel() => sample.GetChannel();
}

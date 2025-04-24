using fluXis.Skinning;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;
using osu.Framework.Utils;

namespace fluXis.Screens.Gameplay;

public partial class GameplaySamples : Component
{
    private Sample restartSample;
    private Sample[] missSamples;
    private Sample failSample;
    private Sample earlyFailSample;

    private SampleChannel failChannel;

    [BackgroundDependencyLoader]
    private void load(ISkin skin, ISampleStore samples)
    {
        restartSample = skin.GetRestartSample();
        missSamples = skin.GetMissSamples();
        failSample = skin.GetFailSample();
        earlyFailSample = samples.Get("Gameplay/fail-no-fail");
    }

    public void Restart() => restartSample?.Play();
    public void Miss() => missSamples?[RNG.Next(0, missSamples.Length)]?.Play();

    public void Fail() => failChannel = failSample?.Play();
    public void CancelFail() => failChannel?.Stop();

    public void EarlyFail() => earlyFailSample?.Play();
}

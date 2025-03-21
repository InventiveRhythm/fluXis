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
    private void load(SkinManager skins, ISampleStore samples)
    {
        restartSample = skins.GetRestartSample();
        missSamples = skins.GetMissSamples();
        failSample = skins.GetFailSample();
        earlyFailSample = samples.Get("Gameplay/fail-no-fail");
    }

    public void Restart() => restartSample?.Play();
    public void Miss() => missSamples?[RNG.Next(0, missSamples.Length)]?.Play();

    public void Fail() => failChannel = failSample?.Play();
    public void CancelFail() => failChannel?.Stop();

    public void EarlyFail() => earlyFailSample?.Play();
}

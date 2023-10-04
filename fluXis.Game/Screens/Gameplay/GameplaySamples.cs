using fluXis.Game.Configuration;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Utils;

namespace fluXis.Game.Screens.Gameplay;

public partial class GameplaySamples : Component
{
    private Bindable<double> hitSoundVolume;

    private Sample hitSample;
    private Sample restartSample;
    private Sample[] missSamples; // yes this is supposed to be an array, skinning will allow for multiple miss sounds

    [BackgroundDependencyLoader]
    private void load(ISampleStore samples, FluXisConfig config)
    {
        hitSoundVolume = config.GetBindable<double>(FluXisSetting.HitSoundVolume);

        hitSample = samples.Get("Gameplay/hitsound");
        restartSample = samples.Get("Gameplay/restart");
        missSamples = new[] { samples.Get("Gameplay/combobreak") };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        hitSoundVolume.BindValueChanged(_ => hitSample.Volume.Value = hitSoundVolume.Value, true);
    }

    public void Hit() => hitSample?.Play();
    public void Restart() => restartSample?.Play();
    public void Miss() => missSamples?[RNG.Next(0, missSamples.Length)]?.Play();
}

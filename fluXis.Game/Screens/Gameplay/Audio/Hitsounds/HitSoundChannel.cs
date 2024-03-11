using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Map.Structures;
using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Utils;

namespace fluXis.Game.Screens.Gameplay.Audio.Hitsounds;

public partial class HitSoundChannel : Component
{
    /// <summary>
    /// See <see cref="Hitsounding.DirectVolume"/>.
    /// </summary>
    public bool DirectVolume { get; set; }

    public string SampleName { get; }
    private Sample sample { get; }

    private Bindable<double> globalVolume { get; }
    private Bindable<double> channelVolume { get; }

    private List<HitSoundFade> fades { get; }

    public HitSoundChannel(string name, Sample sample, Bindable<double> globalVolume, List<HitSoundFade> fades)
    {
        SampleName = name;
        this.sample = sample;
        this.globalVolume = globalVolume;
        channelVolume = new Bindable<double>(1);
        this.fades = fades;
    }

    public void Play()
    {
        var sampleVolume = getSampleVolume();

        sample.Volume.Value = globalVolume.Value * sampleVolume;
        sample?.Play();
    }

    protected override void Update()
    {
        base.Update();

        if (DirectVolume)
            return;

        while (fades.Count > 0 && Clock.CurrentTime > fades[0].Time)
        {
            var fade = fades[0];
            fades.RemoveAt(0);

            this.TransformBindableTo(channelVolume, fade.Volume, fade.Duration, fade.Easing);
        }
    }

    private double getSampleVolume()
    {
        if (!DirectVolume)
            return channelVolume.Value;

        var fade = fades.LastOrDefault(x => x.Time <= Clock.CurrentTime);

        if (fade == null)
            return 1;

        var previous = fades.LastOrDefault(x => x.Time < fade.Time);
        var previousVolume = previous?.Volume ?? 1;

        var time = Clock.CurrentTime - fade.Time;
        var volume = fade.Volume;

        if (time > fade.Duration)
            return volume;

        return Interpolation.ValueAt(Clock.CurrentTime, previousVolume, volume, fade.Time, fade.Time + fade.Duration, fade.Easing);
    }
}

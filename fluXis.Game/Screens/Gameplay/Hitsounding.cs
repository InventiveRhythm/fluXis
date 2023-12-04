using System.Collections.Generic;
using System.IO;
using fluXis.Game.Configuration;
using fluXis.Game.Database;
using fluXis.Game.Database.Maps;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Gameplay;

public partial class Hitsounding : Component
{
    public const string DEFAULT_PREFIX = ":";

    private string[] defaults { get; } =
    {
        "drum",
        "clap"
    };

    private Dictionary<string, Sample> samples { get; } = new();

    private RealmMapSet set { get; }
    private Bindable<double> rate { get; }

    private Bindable<double> volume;

    public Hitsounding(RealmMapSet set, Bindable<double> rate)
    {
        this.set = set;
        this.rate = rate;
    }

    [BackgroundDependencyLoader]
    private void load(ISampleStore defaultSamples, FluXisConfig config)
    {
        volume = config.GetBindable<double>(FluXisSetting.HitSoundVolume);

        foreach (var sample in defaults)
        {
            var s = defaultSamples.Get($"Gameplay/{sample}");
            if (s == null) continue;

            s.Frequency.BindTo(rate);
            samples.Add($"{DEFAULT_PREFIX}{sample}", s);
        }

        var dir = MapFiles.GetFullPath(set.ID.ToString());

        if (!Directory.Exists(dir))
            return;

        foreach (var file in Directory.GetFiles(dir, "*.wav"))
        {
            var name = Path.GetFileNameWithoutExtension(file);
            var s = set.Resources?.SampleStore?.Get($"{set.ID}/{name}");

            if (s == null) continue;

            s.Frequency.BindTo(rate);
            samples.Add(name, s);
        }
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        volume.BindValueChanged(v =>
        {
            foreach (var s in samples.Values)
            {
                if (s == null) continue;

                s.Volume.Value = v.NewValue;
            }
        }, true);
    }

    [CanBeNull]
    public Sample GetSample(string sample)
    {
        if (string.IsNullOrEmpty(sample))
            return null;

        var name = sample.ToLower().EndsWith(".wav") ? Path.GetFileNameWithoutExtension(sample) : sample;
        return samples.TryGetValue(name, out var s) ? s : null;
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        foreach (var s in samples.Values)
        {
            s.Frequency.UnbindAll();
            s.Dispose();
        }
    }
}

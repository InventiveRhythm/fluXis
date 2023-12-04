using System.Collections.Generic;
using System.IO;
using fluXis.Game.Database;
using fluXis.Game.Database.Maps;
using JetBrains.Annotations;
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

    public Hitsounding(ISampleStore defaultSamples, RealmMapSet set, Bindable<double> rate)
    {
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

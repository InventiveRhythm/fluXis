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
    private Dictionary<string, Sample> samples { get; } = new();

    public Hitsounding(RealmMapSet set, Bindable<double> rate)
    {
        var dir = MapFiles.GetFullPath(set.ID.ToString());

        if (!Directory.Exists(dir))
            return;

        foreach (var file in Directory.GetFiles(dir, "*.wav"))
        {
            var name = Path.GetFileNameWithoutExtension(file);
            var channel = set.Resources?.SampleStore?.Get($"{set.ID}/{name}");

            if (channel == null) continue;

            channel.Frequency.BindTo(rate);
            samples.Add(name, channel);
        }
    }

    [CanBeNull]
    public Sample GetSample(string sample)
    {
        if (string.IsNullOrEmpty(sample))
            return null;

        var name = sample.ToLower().EndsWith(".wav") ? Path.GetFileNameWithoutExtension(sample) : sample;
        return samples.TryGetValue(name, out var channel) ? channel : null;
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        foreach (var sample in samples.Values)
        {
            sample.Frequency.UnbindAll();
            sample.Dispose();
        }
    }
}

using System.Collections.Generic;
using System.IO;
using System.Linq;
using fluXis.Game.Configuration;
using fluXis.Game.Database;
using fluXis.Game.Database.Maps;
using fluXis.Game.Map.Structures;
using fluXis.Game.Skinning;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Screens.Gameplay.Audio.Hitsounds;

public partial class Hitsounding : CompositeDrawable
{
    public const string DEFAULT_PREFIX = ":";

    private const string hit_normal = "normal";
    private const string hit_kick = "kick";
    private const string hit_clap = "clap";
    private const string hit_snare = "snare";

    public static IEnumerable<string> Defaults { get; } = new[] { hit_normal, hit_kick, hit_clap, hit_snare };

    private List<HitSoundChannel> channels { get; } = new();

    private RealmMapSet set { get; }
    private List<HitSoundFade> fades { get; }
    private Bindable<double> rate { get; }

    /// <summary>
    /// Does calculations to get the volume of
    /// the hitsound instead of transforms.
    /// </summary>
    public bool DirectVolume { get; init; } = false;

    private Bindable<double> volume;

    public Hitsounding(RealmMapSet set, List<HitSoundFade> fades, Bindable<double> rate)
    {
        this.set = set;
        this.fades = fades;
        this.rate = rate;
    }

    [BackgroundDependencyLoader]
    private void load(ISampleStore defaultSamples, SkinManager skinManager, FluXisConfig config)
    {
        volume = config.GetBindable<double>(FluXisSetting.HitSoundVolume);

        foreach (var sample in Defaults)
        {
            var s = sample == hit_normal
                ? skinManager.GetHitSample()
                : defaultSamples.Get($"Gameplay/{sample}");

            if (s == null)
                continue;

            s.Frequency.BindTo(rate);
            var fade = fades.Where(x => x.HitSound == sample).ToList();
            channels.Add(new HitSoundChannel($"{DEFAULT_PREFIX}{sample}", s, volume, fade));
        }

        var dir = MapFiles.GetFullPath(set.ID.ToString());

        if (Directory.Exists(dir))
        {
            foreach (var file in Directory.GetFiles(dir, "*.wav"))
            {
                var name = Path.GetFileNameWithoutExtension(file);
                var s = set.Resources?.SampleStore?.Get($"{set.ID}/{name}");

                if (s == null)
                    continue;

                s.Frequency.BindTo(rate);
                var fade = fades.Where(x => x.HitSound == name).ToList();
                channels.Add(new HitSoundChannel(name, s, volume, fade));
            }
        }

        channels.ForEach(x => x.DirectVolume = DirectVolume);
        channels.ForEach(AddInternal);
    }

    public HitSoundChannel GetSample(string sample, bool allowCustom = true)
    {
        // should always be the first channel so this is safe
        var defaultChannel = channels.FirstOrDefault();

        if (string.IsNullOrEmpty(sample))
            return defaultChannel;

        if (!allowCustom && !sample.StartsWith(DEFAULT_PREFIX))
            return defaultChannel;

        var name = sample.ToLower().EndsWith(".wav") ? Path.GetFileNameWithoutExtension(sample) : sample;
        return channels.Find(c => c.SampleName == name) ?? defaultChannel;
    }
}

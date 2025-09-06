using System.Collections.Generic;
using System.IO;
using System.Linq;
using fluXis.Configuration;
using fluXis.Database.Maps;
using fluXis.Map.Structures;
using fluXis.Skinning;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Containers;

namespace fluXis.Screens.Gameplay.Audio.Hitsounds;

public partial class Hitsounding : CompositeDrawable
{
    public const string DEFAULT_PREFIX = ":";

    private const string hit_normal = "normal";
    private const string hit_kick = "kick";
    private const string hit_clap = "clap";
    private const string hit_snare = "snare";
    private const string tick_big = "tick-big";
    private const string tick_small = "tick-small";

    public static IEnumerable<string> Defaults { get; } = new[] { hit_normal, hit_kick, hit_clap, hit_snare };

    public Bindable<double> PlayfieldPanning { get; } = new();
    public int PlayfieldCount { get; set; } = 1;

    private List<HitSoundChannel> channels { get; } = new();

    private RealmMapSet set { get; }
    private List<HitSoundFade> fades { get; }
    private Bindable<double> rate { get; }

    /// <summary>
    /// Does calculations to get the volume of
    /// the hitsound instead of transforms.
    /// </summary>
    public bool DirectVolume { get; init; }

    private Bindable<double> userVolume;
    private Bindable<double> volume;

    public Hitsounding(RealmMapSet set, List<HitSoundFade> fades, Bindable<double> rate)
    {
        this.set = set;
        this.fades = fades;
        this.rate = rate;
    }

    [BackgroundDependencyLoader]
    private void load(ISampleStore defaultSamples, ISkin skin, FluXisConfig config)
    {
        userVolume = config.GetBindable<double>(FluXisSetting.HitSoundVolume);
        volume = new Bindable<double>();

        // tick-big and tick-small shouldn't show up in the editor toolbox
        foreach (var sample in Defaults.Concat(new[] { tick_big, tick_small }))
        {
            var s = sample == hit_normal
                ? skin.GetHitSample()
                : defaultSamples.Get($"Gameplay/{sample}");

            if (s == null)
                continue;

            s.Frequency.BindTo(rate);
            s.AddAdjustment(AdjustableProperty.Balance, PlayfieldPanning);
            var fade = fades.Where(x => x.HitSound == sample).ToList();
            channels.Add(new HitSoundChannel($"{DEFAULT_PREFIX}{sample}", s, volume, fade));
        }

        var dir = set.GetPathForFile(".");

        if (Directory.Exists(dir))
        {
            var wav = Directory.GetFiles(dir, "*.wav");
            var ogg = Directory.GetFiles(dir, "*.ogg");

            foreach (var file in wav.Concat(ogg))
            {
                var name = Path.GetFileNameWithoutExtension(file);
                var s = set.Resources?.SampleStore?.Get(set.GetPathForFile(name));

                if (s == null)
                    continue;

                s.Frequency.BindTo(rate);
                s.AddAdjustment(AdjustableProperty.Balance, PlayfieldPanning);
                var fade = fades.Where(x => x.HitSound == name).ToList();
                channels.Add(new HitSoundChannel(name, s, volume, fade));
            }
        }

        channels.ForEach(x => x.DirectVolume = DirectVolume);
        channels.ForEach(AddInternal);
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        userVolume.BindValueChanged(v => volume.Value = v.NewValue * (1 / (float)PlayfieldCount), true);
    }

    public HitSoundChannel GetSample(string sample, bool allowCustom = true)
    {
        // should always be the first channel so this is safe
        var defaultChannel = channels.FirstOrDefault();

        if (string.IsNullOrEmpty(sample))
            return defaultChannel;

        if (!allowCustom && !sample.StartsWith(DEFAULT_PREFIX))
            return defaultChannel;

        var name = sample.ToLower().EndsWith(".wav") || sample.ToLower().EndsWith(".ogg") ? Path.GetFileNameWithoutExtension(sample) : sample;
        return channels.Find(c => c.SampleName == name) ?? defaultChannel;
    }
}

using System.Collections.Generic;
using System.IO;
using System.Linq;
using fluXis.Configuration;
using fluXis.Database;
using fluXis.Database.Maps;
using fluXis.Map.Structures.Events;
using fluXis.Skinning;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Containers;

namespace fluXis.Screens.Gameplay.Audio.Samples;

public partial class HitSamples : CompositeDrawable
{
    public const string DEFAULT_PREFIX = ":";

    private const string hit_normal = "normal";
    private const string hit_kick = "kick";
    private const string hit_clap = "clap";
    private const string hit_snare = "snare";
    private const string tick_big = "tick-big";
    private const string tick_small = "tick-small";

    public static IEnumerable<string> Defaults { get; } = [hit_normal, hit_kick, hit_clap, hit_snare];

    public Bindable<double> PlayfieldPanning { get; } = new();
    public int PlayfieldCount { get; set; } = 1;

    private List<HitSampleChannel> channels { get; } = [];

    private RealmMapSet set { get; }
    private HitSoundFade[] fades { get; }
    private Bindable<double> rate { get; }

    /// <summary>
    /// Does calculations to get the volume of
    /// the sample instead of transforms.
    /// </summary>
    public bool DirectVolume { get; init; }

    private Bindable<double> userVolume;
    private Bindable<double> volume;

    public HitSamples(RealmMapSet set, HitSoundFade[] fades, Bindable<double> rate)
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
            var fade = fades.Where(x => x.HitSound == $"{DEFAULT_PREFIX}{sample}").ToList();
            channels.Add(new HitSampleChannel($"{DEFAULT_PREFIX}{sample}", s, volume, fade));
        }

        foreach (var file in GetFiles())
        {
            var name = Path.GetFileNameWithoutExtension(file);
            var s = set.Resources?.SampleStore?.Get(file);

            if (s == null)
                continue;

            s.Frequency.BindTo(rate);
            s.AddAdjustment(AdjustableProperty.Balance, PlayfieldPanning);
            var fade = fades.Where(x => x.HitSound == name).ToList();
            channels.Add(new HitSampleChannel(name, s, volume, fade));
        }

        channels.ForEach(x => x.DirectVolume = DirectVolume);
        channels.ForEach(AddInternal);
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        userVolume.BindValueChanged(v => volume.Value = v.NewValue * (1 / (float)PlayfieldCount), true);
    }

    public HitSampleChannel GetChannel(string sample, bool allowCustom = true)
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

    public string[] GetFiles(bool relative = false)
    {
        var dir = set.GetPathForFile(".");
        if (!Path.IsPathRooted(dir)) dir = MapFiles.GetFullPath(dir);

        if (!Directory.Exists(dir))
            return [];

        var wav = Directory.GetFiles(dir, "*.wav", SearchOption.AllDirectories);
        var ogg = Directory.GetFiles(dir, "*.ogg", SearchOption.AllDirectories);
        var result = wav.Concat(ogg).Order().Where(x => set.Maps.All(m => !x.EndsWith(m.FullAudioPath))).ToArray();

        if (relative)
            result = [.. result.Select(x => Path.GetRelativePath(dir, x))];

        return result;
    }
}

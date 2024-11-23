using System;
using fluXis.Game.Configuration;
using fluXis.Game.Map.Structures;
using fluXis.Game.Screens.Edit.Tabs.Charting.Effect;
using fluXis.Game.Screens.Edit.Tabs.Charting.Playfield.Tags;
using fluXis.Game.Screens.Edit.Tabs.Shared.Lines;
using fluXis.Game.Screens.Gameplay.Audio.Hitsounds;
using fluXis.Game.Screens.Gameplay.Ruleset;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Audio;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Playfield;

public partial class EditorPlayfield : Container, ITimePositionProvider
{
    [Resolved]
    private EditorSettings settings { get; set; }

    [Resolved]
    private EditorMap map { get; set; }

    [Resolved]
    private EditorClock clock { get; set; }

    [Resolved]
    private FluXisConfig config { get; set; }

    public event Action<string> HitSoundPlayed;

    public EditorHitObjectContainer HitObjectContainer { get; private set; } = new();
    public EditorEffectContainer Effects { get; private set; }
    private WaveformGraph waveform;

    private Sample hitSound;
    private Hitsounding hitsounding;

    [BackgroundDependencyLoader]
    private void load(Bindable<Waveform> waveformBind, ISampleStore samples)
    {
        Width = EditorHitObjectContainer.NOTEWIDTH * map.RealmMap.KeyCount;
        RelativeSizeAxes = Axes.Y;
        Anchor = Origin = Anchor.Centre;

        hitSound = samples.Get("Gameplay/hitsound.mp3");

        InternalChildren = new Drawable[]
        {
            hitsounding = new Hitsounding(map.RealmMap.MapSet, map.MapInfo.HitSoundFades, clock.RateBindable)
            {
                DirectVolume = true,
                Clock = clock
            },
            new Stage(),
            waveform = new WaveformGraph
            {
                Height = EditorHitObjectContainer.NOTEWIDTH * map.RealmMap.KeyCount,
                Anchor = Anchor.BottomRight,
                Origin = Anchor.BottomLeft,
                Rotation = -90,
            },
            Effects = new EditorEffectContainer(),
            new EditorTimingLines(),
            HitObjectContainer,
            new TimingTagContainer(),
            new EffectTagContainer()
        };

        map.KeyModeChanged += count =>
        {
            var newWidth = EditorHitObjectContainer.NOTEWIDTH * count;
            Width = waveform.Height = newWidth;
        };
        waveformBind.BindValueChanged(w => waveform.Waveform = w.NewValue, true);
        settings.WaveformOpacity.BindValueChanged(opacity => waveform.FadeTo(opacity.NewValue, 200), true);
    }

    protected override void Update()
    {
        base.Update();

        float songLengthInPixels = .5f * (clock.TrackLength * settings.Zoom);
        float songTimeInPixels = (float)(-EditorHitObjectContainer.HITPOSITION - .5f * (-(clock.CurrentTime + ChartingContainer.WAVEFORM_OFFSET) * settings.Zoom));

        waveform.Width = songLengthInPixels;
        waveform.Y = songTimeInPixels;

        if (hitSound != null)
            hitSound.Volume.Value = config.Get<double>(FluXisSetting.HitSoundVolume);
    }

    public void PlayHitSound(HitObject info)
    {
        var channel = hitsounding.GetSample(info.HitSound);
        channel.Play();
        HitSoundPlayed?.Invoke(channel.SampleName);
    }

    public double TimeAtScreenSpacePosition(Vector2 pos) => HitObjectContainer.TimeAtScreenSpacePosition(pos);
    public Vector2 ScreenSpacePositionAtTime(double time, int lane) => HitObjectContainer.ScreenSpacePositionAtTime(time, lane);
}

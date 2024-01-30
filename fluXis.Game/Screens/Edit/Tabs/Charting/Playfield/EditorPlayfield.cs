using fluXis.Game.Configuration;
using fluXis.Game.Map.Structures;
using fluXis.Game.Screens.Edit.Tabs.Charting.Effect;
using fluXis.Game.Screens.Edit.Tabs.Charting.Lines;
using fluXis.Game.Screens.Edit.Tabs.Charting.Playfield.Tags;
using fluXis.Game.Screens.Gameplay;
using fluXis.Game.Skinning.Default.Stage;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Audio;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Playfield;

public partial class EditorPlayfield : Container
{
    [Resolved]
    private EditorValues values { get; set; }

    [Resolved]
    private EditorClock clock { get; set; }

    [Resolved]
    private EditorChangeHandler changeHandler { get; set; }

    [Resolved]
    private FluXisConfig config { get; set; }

    public EditorHitObjectContainer HitObjectContainer { get; private set; }
    public EditorEffectContainer Effects { get; private set; }
    private EditorTimingLines timingLines;
    private WaveformGraph waveform;

    private Sample hitSound;
    private Hitsounding hitsounding;

    [BackgroundDependencyLoader]
    private void load(Bindable<Waveform> waveformBind, ISampleStore samples)
    {
        Width = EditorHitObjectContainer.NOTEWIDTH * values.Editor.Map.KeyCount;
        RelativeSizeAxes = Axes.Y;
        Anchor = Origin = Anchor.Centre;

        hitSound = samples.Get("Gameplay/hitsound.mp3");

        InternalChildren = new Drawable[]
        {
            hitsounding = new Hitsounding(values.Editor.Map.MapSet, clock.RateBindable),
            new DefaultStageBackground(),
            new DefaultStageBorderLeft(),
            new DefaultStageBorderRight(),
            waveform = new WaveformGraph
            {
                Height = EditorHitObjectContainer.NOTEWIDTH * values.Editor.Map.KeyCount,
                Anchor = Anchor.BottomRight,
                Origin = Anchor.BottomLeft,
                Rotation = -90,
            },
            Effects = new EditorEffectContainer(),
            timingLines = new EditorTimingLines(),
            HitObjectContainer = new EditorHitObjectContainer(),
            new TimingTagContainer(),
            new EffectTagContainer()
        };

        changeHandler.OnKeyModeChanged += count =>
        {
            var newWidth = EditorHitObjectContainer.NOTEWIDTH * count;
            Width = waveform.Height = newWidth;
        };
        waveformBind.BindValueChanged(w => waveform.Waveform = w.NewValue, true);
        values.WaveformOpacity.BindValueChanged(opacity => waveform.FadeTo(opacity.NewValue, 200), true);
    }

    protected override void Update()
    {
        base.Update();

        float songLengthInPixels = .5f * (clock.TrackLength * values.Zoom);
        float songTimeInPixels = -EditorHitObjectContainer.HITPOSITION - .5f * (-(float)(clock.CurrentTime + ChartingContainer.WAVEFORM_OFFSET) * values.Zoom);

        waveform.Width = songLengthInPixels;
        waveform.Y = songTimeInPixels;

        if (hitSound != null)
            hitSound.Volume.Value = config.Get<double>(FluXisSetting.HitSoundVolume);
    }

    public void PlayHitSound(HitObject info)
    {
        var sample = hitsounding.GetSample(info.HitSound) ?? hitSound;
        sample?.Play();
    }
}

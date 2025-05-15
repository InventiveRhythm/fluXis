using System;
using fluXis.Configuration;
using fluXis.Map.Structures;
using fluXis.Screens.Edit.Tabs.Charting.Effect;
using fluXis.Screens.Edit.Tabs.Charting.Playfield.Tags;
using fluXis.Screens.Edit.Tabs.Shared.Lines;
using fluXis.Screens.Gameplay.Audio.Hitsounds;
using fluXis.Screens.Gameplay.Ruleset;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Audio;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input;
using osuTK;

namespace fluXis.Screens.Edit.Tabs.Charting.Playfield;

public partial class EditorPlayfield : Container, ITimePositionProvider
{
    [Resolved]
    private EditorSettings settings { get; set; }

    [Resolved]
    private EditorMap map { get; set; }

    [Resolved]
    private EditorClock clock { get; set; }

    [Resolved]
    private Hitsounding hitsounding { get; set; }

    [Resolved]
    private FluXisConfig config { get; set; }

    public event Action<string> HitSoundPlayed;

    public bool CursorInPlacementArea => ReceivePositionalInputAt(inputManager.CurrentState.Mouse.Position);
    public int Index { get; }

    private DependencyContainer dependencies;
    private InputManager inputManager;

    public EditorHitObjectContainer HitObjectContainer { get; } = new();
    public EditorEffectContainer Effects { get; private set; }
    private WaveformGraph waveform;

    private Sample hitSound;

    public EditorPlayfield(int idx)
    {
        Index = idx;
    }

    [BackgroundDependencyLoader]
    private void load(Bindable<Waveform> waveformBind, ISampleStore samples)
    {
        dependencies.CacheAs(this);
        dependencies.CacheAs(HitObjectContainer);

        Width = EditorHitObjectContainer.NOTEWIDTH * map.RealmMap.KeyCount;
        RelativeSizeAxes = Axes.Y;
        Anchor = Origin = Anchor.Centre;

        hitSound = samples.Get("Gameplay/hitsound.mp3");

        InternalChildren = new Drawable[]
        {
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
            new Box
            {
                RelativeSizeAxes = Axes.X,
                Height = 5,
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                Y = -EditorHitObjectContainer.HITPOSITION
            },
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

    protected override void LoadComplete()
    {
        base.LoadComplete();

        inputManager = GetContainingInputManager();
    }

    protected override void Update()
    {
        base.Update();

        var songLengthInPixels = .5f * (clock.TrackLength * settings.Zoom);
        var songTimeInPixels = (float)(-EditorHitObjectContainer.HITPOSITION - .5f * (-(clock.CurrentTime + ChartingContainer.WAVEFORM_OFFSET) * settings.Zoom));

        waveform.Width = (float)songLengthInPixels;
        waveform.Y = songTimeInPixels;

        if (hitSound != null)
            hitSound.Volume.Value = config.Get<double>(FluXisSetting.HitSoundVolume);
    }

    public void PlayHitSound(HitObject info, bool force = false)
    {
        if (!clock.IsRunning && !force)
            return;

        var sound = info.HitSound;

        if (sound == ":normal" && info.Type == 1)
        {
            sound = ":tick-big";

            if (info.HoldTime > 0)
                sound = ":tick-small";
        }

        var channel = hitsounding.GetSample(sound);

        if (channel is null)
            return;

        channel.Play();
        HitSoundPlayed?.Invoke(channel.SampleName);
    }

    public float PositionAtTime(double time) => HitObjectContainer.PositionAtTime(time);
    public double TimeAtScreenSpacePosition(Vector2 pos) => HitObjectContainer.TimeAtScreenSpacePosition(pos);
    public Vector2 ScreenSpacePositionAtTime(double time, int lane) => HitObjectContainer.ScreenSpacePositionAtTime(time, lane);

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));
}

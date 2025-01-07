using fluXis.Audio;
using fluXis.Configuration;
using fluXis.Scoring.Processing.Health;
using fluXis.Screens.Gameplay.Ruleset.Playfields;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Screens.Gameplay.UI;

public partial class DangerHealthOverlay : Container
{
    [Resolved]
    private GameplayScreen screen { get; set; }

    [Resolved]
    private PlayfieldManager playfieldManager { get; set; }

    private PlayfieldPlayer player => playfieldManager.FirstPlayer;

    private Bindable<bool> dimOnLowHealth;

    private AudioFilter lowPass;
    private bool exited;

    private Box glow;
    private Box darken;
    private double health = 0;

    private const int threshold = 40;

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config, AudioManager audio)
    {
        dimOnLowHealth = config.GetBindable<bool>(FluXisSetting.DimAndFade);
        dimOnLowHealth.BindValueChanged(onDimOnLowHealthChanged, true);

        RelativeSizeAxes = Axes.Both;

        InternalChildren = new Drawable[]
        {
            lowPass = new AudioFilter(audio.TrackMixer),
            glow = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = ColourInfo.GradientVertical(Colour4.Red.Opacity(0), Colour4.Red.Opacity(.6f)),
                Height = .6f,
                Alpha = 0,
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre
            },
            darken = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Colour4.Black.Opacity(.6f),
                Alpha = 0
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        player.HealthProcessor.Health.BindValueChanged(e => this.TransformTo(nameof(health), e.NewValue, 300, Easing.OutQuint), true);
        screen.OnExit += () =>
        {
            lowPass.CutoffTo(AudioFilter.MAX, FluXisScreen.MOVE_DURATION);
            exited = true;
        };
    }

    private void onDimOnLowHealthChanged(ValueChangedEvent<bool> e)
    {
        if (e.NewValue)
            this.FadeIn(300);
        else
            this.FadeOut(300);
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);
        dimOnLowHealth.ValueChanged -= onDimOnLowHealthChanged;
    }

    protected override void Update()
    {
        base.Update();

        // on easy health, this effect can only be seen at the very end of the song
        if (player.HealthProcessor is RequirementHeathProcessor || !player.HealthProcessor.CanFail)
            return;

        if (player.HealthProcessor.Failed || exited)
            return;

        if (health < threshold && dimOnLowHealth.Value)
        {
            var multiplier = health / threshold;

            darken.Alpha = glow.Alpha = (float)(1 - multiplier);
            lowPass.Cutoff = (int)(AudioFilter.MAX * multiplier);
        }
        else
        {
            darken.Alpha = 0;
            glow.Alpha = 0;

            lowPass.Cutoff = AudioFilter.MAX;
        }
    }
}

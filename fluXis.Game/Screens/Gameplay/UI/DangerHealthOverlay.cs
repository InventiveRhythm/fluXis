using fluXis.Game.Audio;
using fluXis.Game.Configuration;
using fluXis.Game.Scoring.Enums;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Screens.Gameplay.UI;

public partial class DangerHealthOverlay : Container
{
    [Resolved]
    private GameplayScreen screen { get; set; }

    [Resolved]
    private AudioClock clock { get; set; }

    private Bindable<bool> dimOnLowHealth;

    private Box glow;
    private Box darken;
    private float health = 0;

    private const int threshold = 40;

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config)
    {
        dimOnLowHealth = config.GetBindable<bool>(FluXisSetting.DimAndFade);
        dimOnLowHealth.BindValueChanged(onDimOnLowHealthChanged, true);

        RelativeSizeAxes = Axes.Both;

        AddRangeInternal(new Drawable[]
        {
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
        });
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        screen.HealthProcessor.Health.BindValueChanged(e => this.TransformTo(nameof(health), e.NewValue, 300, Easing.OutQuint), true);
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
        dimOnLowHealth.ValueChanged -= onDimOnLowHealthChanged;
    }

    protected override void Update()
    {
        base.Update();

        // on easy health, this effect can only be seen at the very end of the song
        if (screen.Playfield.Manager.HealthMode == HealthMode.Requirement || !screen.HealthProcessor.CanFail)
            return;

        if (screen.HealthProcessor.Failed)
            return;

        if (health < threshold && dimOnLowHealth.Value)
        {
            float multiplier = health / threshold;

            darken.Alpha = 1 - multiplier;
            glow.Alpha = 1 - multiplier;

            clock.LowPassFilter.Cutoff = (int)(LowPassFilter.MAX * multiplier);
        }
        else
        {
            darken.Alpha = 0;
            glow.Alpha = 0;
            clock.LowPassFilter.Cutoff = LowPassFilter.MAX;
        }
    }
}

using fluXis.Game.Audio;
using fluXis.Game.Configuration;
using fluXis.Game.Scoring;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Screens.Gameplay.HUD;

public partial class DangerHealthOverlay : GameplayHUDElement
{
    [Resolved]
    private AudioClock clock { get; set; }

    private Bindable<bool> dimOnLowHealth;

    private readonly Box glow;
    private readonly Box darken;
    private float health;

    private const int threshold = 40;

    public DangerHealthOverlay(GameplayScreen screen)
        : base(screen)
    {
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

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config)
    {
        dimOnLowHealth = config.GetBindable<bool>(FluXisSetting.DimAndFade);
        dimOnLowHealth.BindValueChanged(onDimOnLowHealthChanged, true);
        health = Screen.Playfield.Manager.Health;
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
        if (Screen.Playfield.Manager.HealthMode == HealthMode.Requirement)
            return;

        if (health != Screen.Playfield.Manager.Health)
        {
            this.TransformTo(nameof(health), Screen.Playfield.Manager.Health, 300, Easing.OutQuint);
        }

        if (health < 0)
            health = 0;

        if (Screen.Playfield.Manager.Dead)
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

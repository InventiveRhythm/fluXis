using fluXis.Game.Audio;
using fluXis.Game.Scoring;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Screens.Gameplay.HUD;

public class DangerHealthOverlay : GameplayHUDElement
{
    private readonly Box glow;
    private readonly Box darken;
    private float health;

    private const int threshold = 40;

    public DangerHealthOverlay(GameplayScreen screen)
        : base(screen)
    {
        RelativeSizeAxes = Axes.Both;
        health = screen.Playfield.Manager.Health;

        AddRangeInternal(new Drawable[]
        {
            glow = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = ColourInfo.GradientVertical(Colour4.Red.Opacity(0), Colour4.Red.Opacity(.6f)),
                Height = .6f,
                Alpha = 0,
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
            },
            darken = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Colour4.Black.Opacity(.6f),
                Alpha = 0,
            }
        });
    }

    protected override void Update()
    {
        base.Update();

        // on normal health, this effect can only be seen at the very end of the song
        if (Screen.Playfield.Manager.HealthMode == HealthMode.Normal)
            return;

        if (health != Screen.Playfield.Manager.Health)
        {
            this.TransformTo(nameof(health), Screen.Playfield.Manager.Health, 300, Easing.OutQuint);
        }

        if (health < 0)
            health = 0;

        if (Screen.Playfield.Manager.Dead)
            return;

        if (health < threshold)
        {
            float multiplier = health / threshold;

            darken.Alpha = 1 - multiplier;
            glow.Alpha = 1 - multiplier;

            Conductor.LowPassFilter.Cutoff = (int)(LowPassFilter.MAX * multiplier);
        }
        else
        {
            darken.Alpha = 0;
            glow.Alpha = 0;
            Conductor.LowPassFilter.Cutoff = LowPassFilter.MAX;
        }
    }
}

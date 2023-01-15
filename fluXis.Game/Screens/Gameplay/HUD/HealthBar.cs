using fluXis.Game.Screens.Gameplay.Ruleset;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Screens.Gameplay.HUD;

public class HealthBar : GameplayHUDElement
{
    private readonly HitObjectManager manager;
    private float health;
    private readonly Box bar;

    public HealthBar(GameplayScreen screen)
        : base(screen)
    {
        manager = screen.Playfield.Manager;
        health = manager.Health;

        Anchor = Anchor.BottomCentre;
        Origin = Anchor.BottomCentre;
        CornerRadius = 10;
        Masking = true;
        Y = -40;
        Width = 40;
        Height = 500;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Colour4.White,
                Alpha = .2f,
                Blending = BlendingParameters.Additive
            },
            bar = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Colour4.White,
                Anchor = Anchor.BottomLeft,
                Origin = Anchor.BottomLeft,
            }
        };
    }

    protected override void Update()
    {
        X = Screen.Playfield.Stage.Background.Width / 2 + 40;

        if (manager.Health != health)
            this.TransformTo(nameof(health), manager.Health, 300, Easing.OutQuint);

        bar.Height = health / 100;

        base.Update();
    }
}

using fluXis.Game.Graphics;
using fluXis.Game.Scoring;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Screens.Gameplay.HUD;

public partial class AccuracyDisplay : GameplayHUDElement
{
    private SpriteText accuracyText;
    private float displayedAccuracy;
    private float lastAccuracy;

    private Grade grade = Grade.X;
    private DrawableGrade drawableGrade;

    public AccuracyDisplay(GameplayScreen screen)
        : base(screen)
    {
        displayedAccuracy = 0;
        lastAccuracy = 0;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Anchor = Anchor.Centre;
        Origin = Anchor.TopCentre;

        Add(new FillFlowContainer
        {
            AutoSizeAxes = Axes.Both,
            Direction = FillDirection.Horizontal,
            Anchor = Anchor.Centre,
            Origin = Anchor.TopCentre,
            Children = new Drawable[]
            {
                drawableGrade = new DrawableGrade { Size = 32 },
                accuracyText = new SpriteText { Font = FluXisFont.Default(32, true) }
            }
        });
    }

    protected override void LoadComplete()
    {
        accuracyText.FadeOut().Then(500).FadeInFromZero(250);

        base.LoadComplete();
    }

    protected override void Update()
    {
        var currentAccuracy = Screen.Performance.Accuracy;

        if (currentAccuracy != lastAccuracy)
        {
            float acc = (int)(currentAccuracy * 100) / 100f;
            this.TransformTo(nameof(displayedAccuracy), acc, 400, Easing.Out);
            lastAccuracy = acc;
        }

        accuracyText.Text = $"{displayedAccuracy:00.00}%".Replace(",", ".");

        if (grade != Screen.Performance.Grade)
        {
            grade = Screen.Performance.Grade;
            drawableGrade.Grade = grade;
        }

        base.Update();
    }
}

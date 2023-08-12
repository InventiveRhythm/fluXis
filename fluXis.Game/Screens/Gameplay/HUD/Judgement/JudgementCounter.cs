using fluXis.Game.Graphics;
using fluXis.Game.Scoring;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Screens.Gameplay.HUD.Judgement;

public partial class JudgementCounter : CompositeDrawable
{
    private readonly Performance performance;

    public JudgementCounter(Performance performance)
    {
        this.performance = performance;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Anchor = Anchor.CentreRight;
        Origin = Anchor.CentreRight;
        Width = 50;
        Margin = new MarginPadding { Right = 5 };

        AddInternal(new Container
        {
            Anchor = Anchor.CentreRight,
            Origin = Anchor.CentreRight,
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            CornerRadius = 5,
            Masking = true,
            Children = new Drawable[]
            {
                new Box
                {
                    Colour = FluXisColors.Background4,
                    RelativeSizeAxes = Axes.Both
                },
                new FillFlowContainer<JudgementCounterItem>
                {
                    Direction = FillDirection.Vertical,
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Children = new JudgementCounterItem[]
                    {
                        new(performance, Scoring.Judgement.Flawless),
                        new(performance, Scoring.Judgement.Perfect),
                        new(performance, Scoring.Judgement.Great),
                        new(performance, Scoring.Judgement.Alright),
                        new(performance, Scoring.Judgement.Okay),
                        new(performance, Scoring.Judgement.Miss)
                    }
                }
            }
        });
    }
}

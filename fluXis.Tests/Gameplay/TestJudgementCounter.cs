using fluXis.Scoring;
using fluXis.Scoring.Enums;
using fluXis.Screens.Gameplay.HUD.Components;

namespace fluXis.Tests.Gameplay;

public partial class TestJudgementCounter : FluXisTestScene
{
    public TestJudgementCounter()
    {
        var performance = new ScoreInfo();
        Add(new JudgementCounter());

        int i = 0;

        Scheduler.AddDelayed(() =>
        {
            Judgement toAdd = (Judgement)i;
            // performance.AddJudgement(toAdd);

            i++;
            if (i == 6) i = 0;
        }, 100, true);
    }
}

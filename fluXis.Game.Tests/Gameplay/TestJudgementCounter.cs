using fluXis.Game.Scoring;
using fluXis.Game.Scoring.Enums;
using fluXis.Game.Screens.Gameplay.HUD.Components.Judgement;

namespace fluXis.Game.Tests.Gameplay;

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

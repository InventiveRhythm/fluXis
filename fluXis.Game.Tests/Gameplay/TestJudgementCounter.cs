using fluXis.Game.Map;
using fluXis.Game.Scoring;
using fluXis.Game.Screens.Gameplay.HUD.Judgement;

namespace fluXis.Game.Tests.Gameplay;

public partial class TestJudgementCounter : FluXisTestScene
{
    public TestJudgementCounter()
    {
        var performance = new Performance(new MapInfo(new MapMetadata()));
        Add(new JudgementCounter(performance));

        int i = 0;

        Scheduler.AddDelayed(() =>
        {
            Judgements toAdd = (Judgements)i;
            performance.AddJudgement(toAdd);

            i++;
            if (i == 6) i = 0;
        }, 100, true);
    }
}

using fluXis.Game.Scoring;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

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
        X = -5;

        foreach (var judgement in HitWindow.LIST)
        {
            var counter = new JudgementCounterItem(performance, judgement.Key);
            counter.Y = (int)Height;
            Height += JudgementCounterItem.SIZE + JudgementCounterItem.MARGIN;
            AddInternal(counter);
        }
    }
}
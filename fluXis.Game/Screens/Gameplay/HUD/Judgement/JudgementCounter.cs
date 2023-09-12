using System.Linq;
using fluXis.Game.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Screens.Gameplay.HUD.Judgement;

public partial class JudgementCounter : GameplayHUDElement
{
    [BackgroundDependencyLoader]
    private void load()
    {
        Anchor = Anchor.CentreRight;
        Origin = Anchor.CentreRight;
        Width = 50;

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
                    Children = Screen.HitWindows.GetTimings().Select(t => new JudgementCounterItem(Screen.ScoreProcessor, t)).ToArray()
                }
            }
        });
    }
}

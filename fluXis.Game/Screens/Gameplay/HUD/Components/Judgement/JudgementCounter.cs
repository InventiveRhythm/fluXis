using System.Linq;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Screens.Gameplay.HUD.Components.Judgement;

public partial class JudgementCounter : GameplayHUDComponent
{
    [BackgroundDependencyLoader]
    private void load()
    {
        Width = 50;
        AutoSizeAxes = Axes.Y;
        CornerRadius = 10;
        Masking = true;
        EdgeEffect = FluXisStyles.ShadowSmall;

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
        };
    }
}

using fluXis.Game.Graphics.Drawables;
using fluXis.Game.Graphics.Sprites;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Screens.Gameplay.HUD;

public partial class AccuracyDisplay : GameplayHUDElement
{
    private FluXisSpriteText accuracyText;
    private DrawableScoreRank drawableScoreRank;
    private float accuracy = 0;

    [BackgroundDependencyLoader]
    private void load()
    {
        Anchor = Anchor.Centre;
        Origin = Anchor.TopCentre;

        InternalChild = new FillFlowContainer
        {
            AutoSizeAxes = Axes.Both,
            Direction = FillDirection.Horizontal,
            Anchor = Anchor.Centre,
            Origin = Anchor.TopCentre,
            Children = new Drawable[]
            {
                drawableScoreRank = new DrawableScoreRank { Size = 32 },
                accuracyText = new FluXisSpriteText
                {
                    FontSize = 32,
                    Shadow = true,
                    FixedWidth = true
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        Screen.ScoreProcessor.Accuracy.BindValueChanged(e => this.TransformTo(nameof(accuracy), e.NewValue, 400, Easing.OutQuint), true);
        Screen.ScoreProcessor.Rank.BindValueChanged(e => drawableScoreRank.Rank = e.NewValue, true);
    }

    protected override void Update()
    {
        accuracyText.Text = $"{accuracy:00.00}%".Replace(",", ".");
        base.Update();
    }
}

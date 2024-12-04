using fluXis.Game.Graphics.Drawables;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Scoring.Enums;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Game.Screens.Gameplay.HUD.Components;

public partial class AccuracyDisplay : GameplayHUDComponent
{
    private FluXisSpriteText accuracyText;
    private DrawableScoreRank drawableScoreRank;
    private float accuracy = 0;

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Both;

        InternalChild = new FillFlowContainer
        {
            AutoSizeAxes = Axes.Both,
            Direction = FillDirection.Horizontal,
            Children = new Drawable[]
            {
                drawableScoreRank = new DrawableScoreRank
                {
                    Size = new Vector2(32),
                    FontSize = 32
                },
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

        ScoreProcessor.Accuracy.BindValueChanged(accuracyChanged, true);
        ScoreProcessor.Rank.BindValueChanged(rankChanged, true);
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        ScoreProcessor.Accuracy.ValueChanged -= accuracyChanged;
        ScoreProcessor.Rank.ValueChanged -= rankChanged;
    }

    private void accuracyChanged(ValueChangedEvent<float> e)
    {
        this.TransformTo(nameof(accuracy), e.NewValue, 400, Easing.OutQuint);
    }

    private void rankChanged(ValueChangedEvent<ScoreRank> e)
    {
        drawableScoreRank.Rank = e.NewValue;
    }

    protected override void Update()
    {
        accuracyText.Text = $"{accuracy:00.00}%".Replace(",", ".");
        base.Update();
    }
}

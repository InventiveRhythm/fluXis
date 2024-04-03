using fluXis.Game.Graphics.Drawables;
using fluXis.Game.Graphics.Sprites;
using fluXis.Shared.Scoring.Enums;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

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

        Screen.ScoreProcessor.Accuracy.BindValueChanged(accuracyChanged, true);
        Screen.ScoreProcessor.Rank.BindValueChanged(rankChanged, true);
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        Screen.ScoreProcessor.Accuracy.ValueChanged -= accuracyChanged;
        Screen.ScoreProcessor.Rank.ValueChanged -= rankChanged;
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

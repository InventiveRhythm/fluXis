using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Map;
using fluXis.Game.Skinning;
using fluXis.Shared.Scoring;
using fluXis.Shared.Scoring.Enums;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;
using osuTK.Graphics;

namespace fluXis.Game.Screens.Result.UI;

public partial class ResultScore : FillFlowContainer
{
    public MapInfo MapInfo { get; init; }
    public ScoreInfo Score { get; init; }

    private FluXisSpriteText scoreText;
    private FluXisSpriteText accuracyText;
    private FluXisSpriteText comboText;

    private int score = 0;
    private float accuracy = 0;
    private int combo = 0;

    [BackgroundDependencyLoader]
    private void load(SkinManager skinManager)
    {
        AutoSizeAxes = Axes.Both;
        Direction = FillDirection.Vertical;
        Anchor = Anchor.TopCentre;
        Origin = Anchor.TopCentre;
        Spacing = new Vector2(0, -20);

        AddRange(new Drawable[]
        {
            scoreText = new FluXisSpriteText
            {
                FontSize = 80,
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre
            },
            accuracyText = new FluXisSpriteText
            {
                FontSize = 40,
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre
            },
            new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Direction = FillDirection.Horizontal,
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Spacing = new Vector2(10, 0),
                Margin = new MarginPadding { Top = 20 },
                Children = new Drawable[]
                {
                    new ResultJudgement { Judgement = Judgement.Flawless, Count = Score.Flawless },
                    new ResultJudgement { Judgement = Judgement.Perfect, Count = Score.Perfect },
                    new ResultJudgement { Judgement = Judgement.Great, Count = Score.Great },
                    new ResultJudgement { Judgement = Judgement.Alright, Count = Score.Alright },
                    new ResultJudgement { Judgement = Judgement.Okay, Count = Score.Okay },
                    new ResultJudgement { Judgement = Judgement.Miss, Count = Score.Miss }
                }
            },
            new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Direction = FillDirection.Horizontal,
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Spacing = new Vector2(10, 0),
                Margin = new MarginPadding { Top = 15 },
                Children = new[]
                {
                    new FluXisSpriteText
                    {
                        FontSize = 24,
                        Text = "Max Combo"
                    },
                    comboText = new FluXisSpriteText
                    {
                        FontSize = 24,
                        Colour = Score.FullFlawless ? skinManager.SkinJson.GetColorForJudgement(Judgement.Flawless) : Score.FullCombo ? skinManager.SkinJson.GetColorForJudgement(Judgement.Great) : Color4.White
                    }
                }
            }
        });
    }

    protected override void LoadComplete()
    {
        this.TransformTo(nameof(score), Score.Score, 1000, Easing.OutQuint);
        this.TransformTo(nameof(accuracy), Score.Accuracy, 800, Easing.OutQuint);
        this.TransformTo(nameof(combo), Score.MaxCombo, 800, Easing.OutQuint);

        base.LoadComplete();
    }

    protected override void Update()
    {
        scoreText.Text = score.ToString().PadLeft(7, "0"[0]);
        accuracyText.Text = $"{Score.Rank} - {accuracy:00.00}%".Replace(",", ".");
        comboText.Text = $"{combo}/{MapInfo.MaxCombo}";

        base.Update();
    }
}

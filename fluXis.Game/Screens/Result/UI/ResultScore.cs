using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Scoring;
using fluXis.Game.Skinning;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;
using osuTK.Graphics;

namespace fluXis.Game.Screens.Result.UI;

public partial class ResultScore : FillFlowContainer
{
    public Performance Performance { get; init; }

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

        FillFlowContainer judgementsContainer;

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
            judgementsContainer = new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Direction = FillDirection.Horizontal,
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Spacing = new Vector2(10, 0),
                Margin = new MarginPadding { Top = 20 }
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
                        Colour = Performance.AllFlawless ? skinManager.CurrentSkin.GetColorForJudgement(Judgement.Flawless) : Performance.FullCombo ? skinManager.CurrentSkin.GetColorForJudgement(Judgement.Great) : Color4.White
                    }
                }
            }
        });

        foreach (var judgement in HitWindow.LIST)
        {
            int count = Performance.GetJudgementCount(judgement.Key);
            judgementsContainer.Add(new ResultJudgement { HitWindow = judgement, Count = count });
        }
    }

    protected override void LoadComplete()
    {
        this.TransformTo(nameof(score), Performance.Score, 1000, Easing.OutQuint);
        this.TransformTo(nameof(accuracy), Performance.Accuracy, 800, Easing.OutQuint);
        this.TransformTo(nameof(combo), Performance.MaxCombo, 800, Easing.OutQuint);

        base.LoadComplete();
    }

    protected override void Update()
    {
        scoreText.Text = score.ToString().PadLeft(7, "0"[0]);
        accuracyText.Text = $"{Performance.Grade} - {accuracy:00.00}%".Replace(",", ".");
        comboText.Text = $"{combo}/{Performance.Map.MaxCombo}";

        base.Update();
    }
}

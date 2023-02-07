using fluXis.Game.Scoring;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osuTK;
using osuTK.Graphics;

namespace fluXis.Game.Screens.Result.UI
{
    public partial class ResultScore : FillFlowContainer
    {
        private readonly Performance performance;
        private readonly SpriteText scoreText;
        private readonly SpriteText accuracyText;
        private readonly FillFlowContainer judgementsContainer;
        private readonly SpriteText comboText;

        private int score = 0;
        private float accuracy = 0;
        private int combo = 0;

        public ResultScore(Performance performance)
        {
            this.performance = performance;

            AutoSizeAxes = Axes.Both;
            Direction = FillDirection.Vertical;
            Anchor = Anchor.TopCentre;
            Origin = Anchor.TopCentre;
            Spacing = new Vector2(0, -20);

            AddRange(new Drawable[]
            {
                scoreText = new SpriteText
                {
                    Font = new FontUsage("Quicksand", 80, "Bold"),
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                },
                accuracyText = new SpriteText
                {
                    Font = new FontUsage("Quicksand", 40, "Bold"),
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
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
                        new SpriteText
                        {
                            Font = new FontUsage("Quicksand", 24, "Bold"),
                            Text = "Max Combo"
                        },
                        comboText = new SpriteText
                        {
                            Font = new FontUsage("Quicksand", 24, "SemiBold"),
                            Colour = performance.IsAllFlawless() ? HitWindow.FromKey(Judgements.Flawless).Color : performance.IsFullCombo() ? HitWindow.FromKey(Judgements.Great).Color : Color4.White
                        },
                    }
                }
            });

            foreach (var judgement in HitWindow.LIST)
            {
                int count = performance.GetJudgementCount(judgement.Key);
                judgementsContainer.Add(new ResultJudgement(judgement, count));
            }
        }

        protected override void LoadComplete()
        {
            this.TransformTo(nameof(score), performance.Score, 1000, Easing.OutQuint);
            this.TransformTo(nameof(accuracy), performance.Accuracy, 800, Easing.OutQuint);
            this.TransformTo(nameof(combo), performance.MaxCombo, 800, Easing.OutQuint);

            base.LoadComplete();
        }

        protected override void Update()
        {
            scoreText.Text = score.ToString().PadLeft(7, "0"[0]);
            accuracyText.Text = $"{performance.Grade} - {accuracy:00.00}%".Replace(",", ".");
            comboText.Text = $"{combo}/{performance.Map.MaxCombo}";

            base.Update();
        }
    }
}

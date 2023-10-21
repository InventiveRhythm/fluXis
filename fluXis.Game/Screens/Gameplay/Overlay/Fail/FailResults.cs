using System.Linq;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Scoring.Enums;
using fluXis.Game.Skinning;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Screens.Gameplay.Overlay.Fail;

public partial class FailResults : Container
{
    public FailOverlay FailOverlay { get; set; }

    private FluXisSpriteText title; // title - artist
    private FluXisSpriteText subtitle; // difficulty - mapper
    private FluXisSpriteText scoreText;
    private FluXisSpriteText accuracyText;
    private Box progressBar;
    private FluXisSpriteText progressText;
    private FillFlowContainer<FailResultsJudgement> judgements;

    private float progress;
    private int totalScore;
    private float accuracy;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        Alpha = 0;
        Padding = new MarginPadding(20);

        progress = 0;
        totalScore = 0;
        accuracy = 0;

        InternalChildren = new Drawable[]
        {
            title = new FluXisSpriteText
            {
                FontSize = 32,
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre
            },
            subtitle = new FluXisSpriteText
            {
                FontSize = 24,
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Margin = new MarginPadding { Top = 28 }
            },
            new FluXisSpriteText
            {
                Text = "- FAILED -",
                FontSize = 24,
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Margin = new MarginPadding { Top = 50 },
                Colour = Colour4.FromHex("#FF5555")
            },
            new Container
            {
                AutoSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Children = new Drawable[]
                {
                    new FillFlowContainer
                    {
                        AutoSizeAxes = Axes.Both,
                        Direction = FillDirection.Horizontal,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        Children = new Drawable[]
                        {
                            new Container
                            {
                                AutoSizeAxes = Axes.Y,
                                Width = 250,
                                Children = new Drawable[]
                                {
                                    new FluXisSpriteText
                                    {
                                        Text = "Score",
                                        FontSize = 32,
                                        Anchor = Anchor.TopCentre,
                                        Origin = Anchor.TopCentre
                                    },
                                    scoreText = new FluXisSpriteText
                                    {
                                        FontSize = 64,
                                        Anchor = Anchor.TopCentre,
                                        Origin = Anchor.TopCentre,
                                        Margin = new MarginPadding { Top = 28 }
                                    }
                                }
                            },
                            new Container
                            {
                                AutoSizeAxes = Axes.Y,
                                Width = 250,
                                Children = new Drawable[]
                                {
                                    new FluXisSpriteText
                                    {
                                        Text = "Accuracy",
                                        FontSize = 32,
                                        Anchor = Anchor.TopCentre,
                                        Origin = Anchor.TopCentre
                                    },
                                    accuracyText = new FluXisSpriteText
                                    {
                                        FontSize = 64,
                                        Anchor = Anchor.TopCentre,
                                        Origin = Anchor.TopCentre,
                                        Margin = new MarginPadding { Top = 28 }
                                    }
                                }
                            }
                        }
                    },
                    judgements = new FillFlowContainer<FailResultsJudgement>
                    {
                        Margin = new MarginPadding { Top = 100 },
                        AutoSizeAxes = Axes.Both,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        Spacing = new Vector2(10),
                        Children = FailOverlay.Screen.HitWindows.GetTimings().Select(t => new FailResultsJudgement { Judgement = t.Judgement }).ToArray()
                    }
                }
            },
            new Container
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Anchor = Anchor.BottomLeft,
                Origin = Anchor.BottomLeft,
                Children = new Drawable[]
                {
                    progressText = new FluXisSpriteText
                    {
                        Text = "",
                        FontSize = 32,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre
                    },
                    new CircularContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        Height = 10,
                        Masking = true,
                        Margin = new MarginPadding { Top = 40 },
                        Children = new Drawable[]
                        {
                            new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Alpha = 0.5f
                            },
                            progressBar = new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Width = 0
                            }
                        }
                    }
                }
            }
        };
    }

    protected override void Update()
    {
        progressText.Text = $"{progress:P0}";
        progressBar.Width = progress;
        scoreText.Text = $"{totalScore:0000000}";
        accuracyText.Text = $"{accuracy / 100:P2}".Replace(",", ".");
    }

    public override void Show()
    {
        this.FadeIn(200);

        var map = FailOverlay.Screen.Map;
        var score = FailOverlay.Screen.ScoreProcessor.ToScoreInfo();

        title.Text = $"{map.Metadata.Title} - {map.Metadata.Artist}";
        subtitle.Text = $"[{map.Metadata.Difficulty}] mapped by {map.Metadata.Mapper}";

        float start = map.StartTime;
        float end = map.EndTime - start;
        float current = (float)FailOverlay.Screen.HealthProcessor.FailTime - start;

        float p = current / end;
        this.TransformTo(nameof(progress), p, 2000, Easing.OutQuint);
        this.TransformTo(nameof(totalScore), score.Score, 2000, Easing.OutQuint);
        this.TransformTo(nameof(accuracy), score.Accuracy, 2000, Easing.OutQuint);

        foreach (var judgement in judgements)
        {
            judgement.JudgementCount = judgement.Judgement switch
            {
                Judgement.Flawless => score.Flawless,
                Judgement.Perfect => score.Perfect,
                Judgement.Great => score.Great,
                Judgement.Alright => score.Alright,
                Judgement.Okay => score.Okay,
                Judgement.Miss => score.Miss,
                _ => 0
            };
        }
    }

    private partial class FailResultsJudgement : FillFlowContainer
    {
        private FluXisSpriteText countText;
        private int count = 0;

        public Judgement Judgement { get; init; }

        public int JudgementCount
        {
            set => this.TransformTo(nameof(count), value, 2000, Easing.OutQuint);
        }

        [BackgroundDependencyLoader]
        private void load(SkinManager skinManager)
        {
            AutoSizeAxes = Axes.Both;
            Direction = FillDirection.Horizontal;
            Spacing = new Vector2(5, 0);

            InternalChildren = new Drawable[]
            {
                new FluXisSpriteText
                {
                    FontSize = 32,
                    Text = Judgement.ToString(),
                    Colour = skinManager.SkinJson.GetColorForJudgement(Judgement)
                },
                countText = new FluXisSpriteText
                {
                    FontSize = 32
                }
            };
        }

        protected override void Update()
        {
            countText.Text = $"{count}";
            base.Update();
        }
    }
}

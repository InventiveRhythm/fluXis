using fluXis.Game.Graphics;
using fluXis.Game.Scoring;
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

    private readonly FluXisSpriteText title; // title - artist
    private readonly FluXisSpriteText subtitle; // difficulty - mapper
    private readonly FluXisSpriteText scoreText;
    private readonly FluXisSpriteText accuracyText;
    private readonly Box progressBar;
    private readonly FluXisSpriteText progressText;
    private readonly FillFlowContainer<FailResultsJudgement> judgements;

    private float progress;
    private int score;
    private float accuracy;

    public FailResults()
    {
        RelativeSizeAxes = Axes.Both;
        Alpha = 0;
        Padding = new MarginPadding(20);

        progress = 0;
        score = 0;
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
                        Children = new[]
                        {
                            new FailResultsJudgement { HitWindow = HitWindow.FromKey(Judgement.Flawless) },
                            new FailResultsJudgement { HitWindow = HitWindow.FromKey(Judgement.Perfect) },
                            new FailResultsJudgement { HitWindow = HitWindow.FromKey(Judgement.Great) },
                            new FailResultsJudgement { HitWindow = HitWindow.FromKey(Judgement.Alright) },
                            new FailResultsJudgement { HitWindow = HitWindow.FromKey(Judgement.Okay) },
                            new FailResultsJudgement { HitWindow = HitWindow.FromKey(Judgement.Miss) }
                        }
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
        scoreText.Text = $"{score:0000000}";
        accuracyText.Text = $"{accuracy / 100:P2}".Replace(",", ".");
    }

    public override void Show()
    {
        this.FadeIn(200);

        var map = FailOverlay.Screen.Map;
        var performance = FailOverlay.Screen.Performance;

        title.Text = $"{map.Metadata.Title} - {map.Metadata.Artist}";
        subtitle.Text = $"[{map.Metadata.Difficulty}] mapped by {map.Metadata.Mapper}";

        float start = map.StartTime;
        float end = map.EndTime - start;
        float current = FailOverlay.Screen.DeathTime - start;

        float p = current / end;
        this.TransformTo(nameof(progress), p, 2000, Easing.OutQuint);
        this.TransformTo(nameof(score), performance.Score, 2000, Easing.OutQuint);
        this.TransformTo(nameof(accuracy), performance.Accuracy, 2000, Easing.OutQuint);

        foreach (var judgement in judgements)
        {
            judgement.JudgementCount = performance.Judgements.TryGetValue(judgement.HitWindow.Key, out var performanceJudgement)
                ? performanceJudgement
                : 0;
        }
    }

    private partial class FailResultsJudgement : FillFlowContainer
    {
        private FluXisSpriteText name;
        private FluXisSpriteText countText;
        private int count = 0;

        public HitWindow HitWindow { set; get; }

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
                name = new FluXisSpriteText
                {
                    FontSize = 32,
                    Text = HitWindow.Key.ToString(),
                    Colour = skinManager.CurrentSkin.GetColorForJudgement(HitWindow.Key)
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

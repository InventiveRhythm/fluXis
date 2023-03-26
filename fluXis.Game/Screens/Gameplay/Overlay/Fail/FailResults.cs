using fluXis.Game.Scoring;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Game.Screens.Gameplay.Overlay.Fail;

public partial class FailResults : Container
{
    public FailOverlay FailOverlay { get; set; }

    private readonly SpriteText title; // title - artist
    private readonly SpriteText subtitle; // difficulty - mapper
    private readonly SpriteText scoreText;
    private readonly SpriteText accuracyText;
    private readonly Box progressBar;
    private readonly SpriteText progressText;
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
            title = new SpriteText
            {
                Font = new FontUsage("Quicksand", 32, "Bold"),
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre
            },
            subtitle = new SpriteText
            {
                Font = new FontUsage("Quicksand", 24, "Bold"),
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Margin = new MarginPadding { Top = 28 }
            },
            new SpriteText
            {
                Text = "- FAILED -",
                Font = new FontUsage("Quicksand", 24, "Bold"),
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
                                Width = 200,
                                Children = new Drawable[]
                                {
                                    new SpriteText
                                    {
                                        Text = "Score",
                                        Font = new FontUsage("Quicksand", 32, "Bold"),
                                        Anchor = Anchor.TopCentre,
                                        Origin = Anchor.TopCentre
                                    },
                                    scoreText = new SpriteText
                                    {
                                        Font = new FontUsage("Quicksand", 64, "Bold"),
                                        Anchor = Anchor.TopCentre,
                                        Origin = Anchor.TopCentre,
                                        Margin = new MarginPadding { Top = 28 }
                                    }
                                }
                            },
                            new Container
                            {
                                AutoSizeAxes = Axes.Y,
                                Width = 200,
                                Children = new Drawable[]
                                {
                                    new SpriteText
                                    {
                                        Text = "Accuracy",
                                        Font = new FontUsage("Quicksand", 32, "Bold"),
                                        Anchor = Anchor.TopCentre,
                                        Origin = Anchor.TopCentre
                                    },
                                    accuracyText = new SpriteText
                                    {
                                        Font = new FontUsage("Quicksand", 64, "Bold"),
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
                    progressText = new SpriteText
                    {
                        Text = "",
                        Font = new FontUsage("Quicksand", 32, "Bold"),
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
            judgement.JudgementCount = performance.Judgements.ContainsKey(judgement.HitWindow.Key)
                ? performance.Judgements[judgement.HitWindow.Key]
                : 0;
        }
    }

    private partial class FailResultsJudgement : FillFlowContainer
    {
        private readonly SpriteText name;
        private readonly SpriteText countText;
        private int count = 0;

        private HitWindow hitWindow;

        public HitWindow HitWindow
        {
            set
            {
                name.Text = value.Key.ToString();
                name.Colour = value.Color;
                hitWindow = value;
            }
            get => hitWindow;
        }

        public int JudgementCount
        {
            set => this.TransformTo(nameof(count), value, 2000, Easing.OutQuint);
        }

        public FailResultsJudgement()
        {
            AutoSizeAxes = Axes.Both;
            Direction = FillDirection.Horizontal;
            Spacing = new Vector2(5, 0);

            InternalChildren = new Drawable[]
            {
                name = new SpriteText
                {
                    Font = new FontUsage("Quicksand", 32, "Bold"),
                },
                countText = new SpriteText
                {
                    Font = new FontUsage("Quicksand", 32, "SemiBold")
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

using System.Linq;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Drawables;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Map.Drawables;
using fluXis.Game.Mods.Drawables;
using fluXis.Game.Online.API;
using fluXis.Game.Screens.Result.UI;
using fluXis.Game.Skinning;
using fluXis.Game.Utils;
using fluXis.Shared.Components.Users;
using fluXis.Shared.Scoring;
using fluXis.Shared.Scoring.Enums;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Screens.Result.Normal;

public partial class NormalResults : Container
{
    [Resolved]
    private ScoreInfo score { get; set; }

    [Resolved]
    private RealmMap map { get; set; }

    [Resolved]
    private APIUser player { get; set; }

    [Resolved]
    private SoloResults results { get; set; }

    [Resolved]
    private SkinManager skins { get; set; }

    private FluXisSpriteText completionText;
    private FluXisSpriteText scoreDifferenceText;

    private Container bannerContainer;
    private Container avatarContainer;
    private FluXisSpriteText playedAt;

    private OnlineStatistic ptrStat;
    private OnlineStatistic ovrStat;
    private OnlineStatistic rankStat;
    private FillFlowContainer onlineStats;
    private FluXisSpriteText errorText;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        Anchor = Origin = Anchor.Centre;

        InternalChildren = new Drawable[]
        {
            new Container
            {
                Size = new Vector2(1250, 700),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Children = new Drawable[]
                {
                    new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        CornerRadius = 20,
                        Masking = true,
                        EdgeEffect = FluXisStyles.ShadowMedium,
                        Child = new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = FluXisColors.Background2
                        }
                    },
                    new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        Direction = FillDirection.Vertical,
                        Children = new Drawable[]
                        {
                            new Container
                            {
                                Size = new Vector2(1300, 100),
                                Anchor = Anchor.TopCentre,
                                Origin = Anchor.TopCentre,
                                Margin = new MarginPadding { Bottom = 90 },
                                Children = new Drawable[]
                                {
                                    new Container
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Width = 0.5f,
                                        Anchor = Anchor.CentreLeft,
                                        Origin = Anchor.CentreLeft,
                                        CornerRadius = 20,
                                        Masking = true,
                                        EdgeEffect = FluXisStyles.ShadowSmall,
                                        Children = new[]
                                        {
                                            new MapBackground(map)
                                            {
                                                RelativeSizeAxes = Axes.Both,
                                                Anchor = Anchor.Centre,
                                                Origin = Anchor.Centre
                                            },
                                            getGradient(Colour4.Black, false),
                                            getGradient(string.IsNullOrEmpty(map.Metadata.ColorHex) ? Colour4.Black : Colour4.FromHex(map.Metadata.ColorHex), false),
                                            new FillFlowContainer
                                            {
                                                RelativeSizeAxes = Axes.Both,
                                                Direction = FillDirection.Vertical,
                                                Padding = new MarginPadding { Horizontal = 20, Vertical = 10 },
                                                Spacing = new Vector2(-3),
                                                Children = new Drawable[]
                                                {
                                                    new TruncatingText
                                                    {
                                                        RelativeSizeAxes = Axes.X,
                                                        Anchor = Anchor.CentreLeft,
                                                        Origin = Anchor.CentreLeft,
                                                        Text = map.Difficulty,
                                                        Shadow = true,
                                                        FontSize = 24
                                                    },
                                                    new FluXisSpriteText
                                                    {
                                                        Anchor = Anchor.CentreLeft,
                                                        Origin = Anchor.CentreLeft,
                                                        Text = $"mapped by {map.Metadata.Mapper}",
                                                        Alpha = .8f,
                                                        Shadow = true,
                                                        FontSize = 18
                                                    },
                                                    new DifficultyChip
                                                    {
                                                        Size = new Vector2(80, 20),
                                                        Anchor = Anchor.CentreLeft,
                                                        Origin = Anchor.CentreLeft,
                                                        Margin = new MarginPadding { Top = 5 },
                                                        Rating = map.Filters.NotesPerSecond,
                                                        EdgeEffect = FluXisStyles.ShadowSmall
                                                    }
                                                }
                                            }
                                        }
                                    },
                                    new Container
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Width = 0.5f,
                                        Anchor = Anchor.CentreRight,
                                        Origin = Anchor.CentreRight,
                                        CornerRadius = 20,
                                        Masking = true,
                                        EdgeEffect = FluXisStyles.ShadowSmall,
                                        Children = new[]
                                        {
                                            new Box
                                            {
                                                RelativeSizeAxes = Axes.Both,
                                                Colour = FluXisColors.Background3
                                            },
                                            bannerContainer = new Container
                                            {
                                                RelativeSizeAxes = Axes.Both,
                                            },
                                            getGradient(Colour4.Black, true),
                                            new FillFlowContainer
                                            {
                                                AutoSizeAxes = Axes.X,
                                                RelativeSizeAxes = Axes.Y,
                                                Direction = FillDirection.Horizontal,
                                                Anchor = Anchor.CentreRight,
                                                Origin = Anchor.CentreRight,
                                                Padding = new MarginPadding(10),
                                                Spacing = new Vector2(10),
                                                Children = new Drawable[]
                                                {
                                                    avatarContainer = new Container
                                                    {
                                                        Size = new Vector2(80),
                                                        Anchor = Anchor.CentreRight,
                                                        Origin = Anchor.CentreRight,
                                                        CornerRadius = 10,
                                                        Masking = true,
                                                        EdgeEffect = FluXisStyles.ShadowSmall
                                                    },
                                                    new FillFlowContainer
                                                    {
                                                        AutoSizeAxes = Axes.Both,
                                                        Direction = FillDirection.Vertical,
                                                        Anchor = Anchor.CentreRight,
                                                        Origin = Anchor.CentreRight,
                                                        Children = new Drawable[]
                                                        {
                                                            new FluXisSpriteText
                                                            {
                                                                Anchor = Anchor.CentreRight,
                                                                Origin = Anchor.CentreRight,
                                                                Text = "played by",
                                                                FontSize = 18,
                                                                Shadow = true,
                                                                Alpha = .8f
                                                            },
                                                            new FluXisSpriteText
                                                            {
                                                                Anchor = Anchor.CentreRight,
                                                                Origin = Anchor.CentreRight,
                                                                Text = player.Username,
                                                                FontSize = 28,
                                                                Shadow = true
                                                            },
                                                            playedAt = new FluXisSpriteText
                                                            {
                                                                Anchor = Anchor.CentreRight,
                                                                Origin = Anchor.CentreRight,
                                                                FontSize = 18,
                                                                Shadow = true,
                                                                Alpha = .8f
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    },
                                    new Container
                                    {
                                        Size = new Vector2(250),
                                        Anchor = Anchor.Centre,
                                        Origin = Anchor.Centre,
                                        CornerRadius = 20,
                                        Masking = true,
                                        EdgeEffect = FluXisStyles.ShadowMedium,
                                        Child = new MapCover(map.MapSet)
                                        {
                                            RelativeSizeAxes = Axes.Both,
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre
                                        }
                                    }
                                }
                            },
                            new FillFlowContainer
                            {
                                RelativeSizeAxes = Axes.X,
                                Height = 80,
                                Direction = FillDirection.Vertical,
                                Padding = new MarginPadding { Horizontal = 20 },
                                Children = new Drawable[]
                                {
                                    new TruncatingText
                                    {
                                        MaxWidth = 800,
                                        Anchor = Anchor.Centre,
                                        Origin = Anchor.Centre,
                                        Text = map.Metadata.Title,
                                        FontSize = 36
                                    },
                                    new TruncatingText
                                    {
                                        MaxWidth = 800,
                                        Anchor = Anchor.Centre,
                                        Origin = Anchor.Centre,
                                        Text = map.Metadata.Artist,
                                        FontSize = 24
                                    }
                                }
                            },
                            new FillFlowContainer
                            {
                                RelativeSizeAxes = Axes.X,
                                Height = 310,
                                Direction = FillDirection.Horizontal,
                                Children = new Drawable[]
                                {
                                    new FillFlowContainer
                                    {
                                        Width = 250,
                                        RelativeSizeAxes = Axes.Y,
                                        Anchor = Anchor.Centre,
                                        Origin = Anchor.Centre,
                                        Direction = FillDirection.Vertical,
                                        Spacing = new Vector2(10),
                                        Children = new Drawable[]
                                        {
                                            new Entry
                                            {
                                                Title = "Accuracy",
                                                Value = $"{score.Accuracy.ToStringInvariant("00.00")}%",
                                            },
                                            new MaxComboEntry
                                            {
                                                Title = "Max Combo",
                                                MaxCombo = score.MaxCombo,
                                                TotalNotes = map.Filters.NoteCount + map.Filters.LongNoteCount * 2
                                            },
                                            new Entry
                                            {
                                                Title = "Performance Rating",
                                                Value = score.PerformanceRating > 0 ? $"{score.PerformanceRating.ToStringInvariant("00.00")}pr" : "--.--pr"
                                            },
                                            new Entry
                                            {
                                                Title = "Scroll Speed",
                                                Value = score.ScrollSpeed.ToStringInvariant("0.0")
                                            }
                                        }
                                    },
                                    new FillFlowContainer
                                    {
                                        Width = 550,
                                        RelativeSizeAxes = Axes.Y,
                                        Anchor = Anchor.Centre,
                                        Origin = Anchor.Centre,
                                        Direction = FillDirection.Vertical,
                                        Spacing = new Vector2(20),
                                        Children = new Drawable[]
                                        {
                                            completionText = new FluXisSpriteText
                                            {
                                                Anchor = Anchor.Centre,
                                                Origin = Anchor.Centre,
                                                FontSize = 36
                                            },
                                            new FillFlowContainer
                                            {
                                                AutoSizeAxes = Axes.Both,
                                                Direction = FillDirection.Vertical,
                                                Anchor = Anchor.Centre,
                                                Origin = Anchor.Centre,
                                                Spacing = new Vector2(-10),
                                                Children = new Drawable[]
                                                {
                                                    new FluXisSpriteText
                                                    {
                                                        Anchor = Anchor.TopCentre,
                                                        Origin = Anchor.TopCentre,
                                                        Text = "Score",
                                                        FontSize = 28,
                                                        Colour = FluXisColors.Text2
                                                    },
                                                    new FluXisSpriteText
                                                    {
                                                        Anchor = Anchor.TopCentre,
                                                        Origin = Anchor.TopCentre,
                                                        Text = score.Score.ToString("0000000"),
                                                        FontSize = 68
                                                    },
                                                    scoreDifferenceText = new FluXisSpriteText
                                                    {
                                                        Anchor = Anchor.TopCentre,
                                                        Origin = Anchor.TopCentre,
                                                        FontSize = 28
                                                    }
                                                }
                                            },
                                            new ModList
                                            {
                                                AutoSizeAxes = Axes.X,
                                                Height = 40,
                                                Anchor = Anchor.Centre,
                                                Origin = Anchor.Centre,
                                                Mods = score.Mods.Select(ModUtils.GetFromAcronym).ToList()
                                            }
                                        }
                                    },
                                    new FillFlowContainer
                                    {
                                        Width = 250,
                                        RelativeSizeAxes = Axes.Y,
                                        Anchor = Anchor.Centre,
                                        Origin = Anchor.Centre,
                                        Direction = FillDirection.Vertical,
                                        Spacing = new Vector2(10),
                                        Children = new Drawable[]
                                        {
                                            new Entry
                                            {
                                                Title = "Flawless",
                                                Value = $"{score.Flawless}",
                                                Right = true,
                                                Colour = skins.SkinJson.GetColorForJudgement(Judgement.Flawless)
                                            },
                                            new Entry
                                            {
                                                Title = "Perfect",
                                                Value = $"{score.Perfect}",
                                                Right = true,
                                                Colour = skins.SkinJson.GetColorForJudgement(Judgement.Perfect)
                                            },
                                            new Entry
                                            {
                                                Title = "Great",
                                                Value = $"{score.Great}",
                                                Right = true,
                                                Colour = skins.SkinJson.GetColorForJudgement(Judgement.Great)
                                            },
                                            new Entry
                                            {
                                                Title = "Alright",
                                                Value = $"{score.Alright}",
                                                Right = true,
                                                Colour = skins.SkinJson.GetColorForJudgement(Judgement.Alright)
                                            },
                                            new Entry
                                            {
                                                Title = "Okay",
                                                Value = $"{score.Okay}",
                                                Right = true,
                                                Colour = skins.SkinJson.GetColorForJudgement(Judgement.Okay)
                                            },
                                            new Entry
                                            {
                                                Title = "Miss",
                                                Value = $"{score.Miss}",
                                                Right = true,
                                                Colour = skins.SkinJson.GetColorForJudgement(Judgement.Miss)
                                            }
                                        }
                                    }
                                }
                            },
                            new Container
                            {
                                RelativeSizeAxes = Axes.X,
                                Height = 120,
                                Children = new Drawable[]
                                {
                                    onlineStats = new FillFlowContainer
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Direction = FillDirection.Horizontal,
                                        Alpha = 0,
                                        Children = new Drawable[]
                                        {
                                            ovrStat = new OnlineStatistic { Title = "Overall Rating" },
                                            ptrStat = new OnlineStatistic { Title = "Potential Rating" },
                                            rankStat = new OnlineStatistic { Title = "Global Rank", DisplayAsRank = true }
                                        }
                                    },
                                    errorText = new FluXisSpriteText
                                    {
                                        Anchor = Anchor.Centre,
                                        Origin = Anchor.Centre,
                                        Alpha = 0,
                                        Text = "Failed to update statistics!",
                                        FontSize = 24
                                    }
                                }
                            }
                        }
                    }
                }
            },
            new ResultsScrollForMore
            {
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                Margin = new MarginPadding { Bottom = 60 }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        if (score.FullFlawless)
        {
            completionText.Text = "FULL FLAWLESS";
            completionText.Colour = skins.SkinJson.GetColorForJudgement(Judgement.Flawless);
        }
        else if (score.FullCombo)
        {
            completionText.Text = "FULL COMBO";
            completionText.Colour = skins.SkinJson.GetColorForJudgement(Judgement.Perfect);
        }
        else
        {
            completionText.Text = "CLEARED";
            completionText.Colour = FluXisColors.Accent2.Lighten(.4f);
        }

        if (results.ComparisonScore != null)
        {
            var difference = score.Score - results.ComparisonScore.Score;
            scoreDifferenceText.Text = difference.ToString("+#0;-#0");
        }

        var date = TimeUtils.GetFromSeconds(score.Timestamp);
        playedAt.Text = $"{date.Day} {date:MMM} {date.Year} @ {date:HH:mm}";

        if (results.SubmitRequest is not null)
            results.SubmitRequest.Success += _ => updateStats();

        // this is called here for the case the request is null
        updateStats();

        LoadComponentAsync(new DrawableAvatar(player)
        {
            RelativeSizeAxes = Axes.Both,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            FillMode = FillMode.Fill
        }, avatarContainer.Add);

        LoadComponentAsync(new DrawableBanner(player)
        {
            RelativeSizeAxes = Axes.Both,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            FillMode = FillMode.Fill
        }, bannerContainer.Add);
    }

    private void updateStats()
    {
        if (results.SubmitRequest is not null)
        {
            var res = results.SubmitRequest.Response;
            if (res is null) return;

            if (res.Success)
            {
                ptrStat.Value = res.Data.PotentialRating;
                ptrStat.Difference = res.Data.PotentialRatingChange;

                ovrStat.Value = res.Data.OverallRating;
                ovrStat.Difference = res.Data.OverallRatingChange;

                rankStat.Value = res.Data.Rank;
                rankStat.Difference = res.Data.RankChange;

                onlineStats.FadeIn(200);
            }
            else
            {
                errorText.Text = results.SubmitRequest.FailReason?.Message ?? APIRequest.UNKNOWN_ERROR;
                errorText.FadeIn(200);
            }
        }
        else
        {
            errorText.Text = "No ranking data available!";
            errorText.FadeIn(200);
        }
    }

    private Drawable getGradient(Colour4 color, bool right)
    {
        var gradient = ColourInfo.GradientHorizontal(right ? color.Opacity(0) : color, right ? color : color.Opacity(0));

        return new Container
        {
            RelativeSizeAxes = Axes.Both,
            Alpha = .5f,
            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Width = .8f,
                    Colour = gradient,
                    Anchor = right ? Anchor.CentreLeft : Anchor.CentreRight,
                    Origin = right ? Anchor.CentreLeft : Anchor.CentreRight
                },
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Width = .2f,
                    Colour = color,
                    Anchor = right ? Anchor.CentreRight : Anchor.CentreLeft,
                    Origin = right ? Anchor.CentreRight : Anchor.CentreLeft
                }
            }
        };
    }

    private partial class Entry : Container
    {
        public string Title { get; init; } = "Title";
        public string Value { get; init; } = "Value";
        public bool Right { get; init; }

        [BackgroundDependencyLoader]
        private void load()
        {
            Size = new Vector2(190, 40);
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;

            InternalChildren = new Drawable[]
            {
                new CircularContainer
                {
                    Width = 6,
                    RelativeSizeAxes = Axes.Y,
                    Anchor = Right ? Anchor.CentreRight : Anchor.CentreLeft,
                    Origin = Right ? Anchor.CentreRight : Anchor.CentreLeft,
                    Masking = true,
                    Child = new Box { RelativeSizeAxes = Axes.Both }
                },
                new FillFlowContainer
                {
                    AutoSizeAxes = Axes.Both,
                    Direction = FillDirection.Vertical,
                    Anchor = Right ? Anchor.CentreRight : Anchor.CentreLeft,
                    Origin = Right ? Anchor.CentreRight : Anchor.CentreLeft,
                    Margin = new MarginPadding { Horizontal = 6 },
                    Padding = new MarginPadding { Horizontal = 10 },
                    Spacing = new Vector2(-3),
                    Children = new[]
                    {
                        new FluXisSpriteText
                        {
                            Anchor = Right ? Anchor.CentreRight : Anchor.CentreLeft,
                            Origin = Right ? Anchor.CentreRight : Anchor.CentreLeft,
                            Text = Title,
                            FontSize = 18,
                            Alpha = .8f
                        },
                        CreateBottomRow().With(d =>
                        {
                            d.Anchor = d.Origin = Right ? Anchor.CentreRight : Anchor.CentreLeft;
                        })
                    }
                }
            };
        }

        protected virtual Drawable CreateBottomRow()
        {
            return new FluXisSpriteText
            {
                Anchor = Right ? Anchor.CentreRight : Anchor.CentreLeft,
                Origin = Right ? Anchor.CentreRight : Anchor.CentreLeft,
                FontSize = 24,
                Text = Value
            };
        }
    }

    private partial class MaxComboEntry : Entry
    {
        public int MaxCombo { get; init; }
        public int TotalNotes { get; init; }

        protected override Drawable CreateBottomRow()
        {
            return new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Direction = FillDirection.Horizontal,
                Children = new Drawable[]
                {
                    new FluXisSpriteText
                    {
                        Anchor = Anchor.BottomLeft,
                        Origin = Anchor.BottomLeft,
                        FontSize = 24,
                        Text = $"{MaxCombo}"
                    },
                    new FluXisSpriteText
                    {
                        Anchor = Anchor.BottomLeft,
                        Origin = Anchor.BottomLeft,
                        FontSize = 18,
                        Alpha = .8f,
                        Text = $"/{TotalNotes}x"
                    }
                }
            };
        }
    }

    private partial class OnlineStatistic : FillFlowContainer
    {
        public string Title { get; init; } = "Title";
        public bool DisplayAsRank { get; init; }

        public double Value
        {
            set => valueText.Text = DisplayAsRank ? $"#{value}" : value.ToStringInvariant("00.00");
        }

        public double Difference
        {
            set
            {
                var negativeColor = Colour4.FromHex("#FF5555");
                var positiveColor = Colour4.FromHex("#55FF55");

                switch (value)
                {
                    case > 0:
                        differenceText.Text = DisplayAsRank ? $"+{value}" : $"+{value.ToStringInvariant("00.00")}";
                        differenceText.Colour = DisplayAsRank ? negativeColor : positiveColor;
                        break;

                    case < 0:
                        differenceText.Text = DisplayAsRank ? $"{value}" : $"{value.ToStringInvariant("00.00")}";
                        differenceText.Colour = DisplayAsRank ? positiveColor : negativeColor;
                        break;

                    default:
                        differenceText.Text = "KEEP";
                        differenceText.Colour = FluXisColors.Text2;
                        break;
                }
            }
        }

        private FluXisSpriteText valueText;
        private FluXisSpriteText differenceText;

        [BackgroundDependencyLoader]
        private void load()
        {
            Width = 200;
            RelativeSizeAxes = Axes.Y;
            Direction = FillDirection.Vertical;
            Anchor = Origin = Anchor.Centre;
            Spacing = new Vector2(-5);

            InternalChildren = new Drawable[]
            {
                new FluXisSpriteText
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Text = Title,
                    FontSize = 14,
                    Alpha = .8f,
                    Margin = new MarginPadding { Bottom = 2 }
                },
                valueText = new FluXisSpriteText
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    FontSize = 32
                },
                differenceText = new FluXisSpriteText
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    FontSize = 20
                }
            };
        }
    }
}

using System;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Graphics.Drawables;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Text;
using fluXis.Game.Map.Drawables.Online;
using fluXis.Game.Online.API.Models.Scores;
using fluXis.Game.Scoring.Enums;
using fluXis.Game.Screens;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Overlay.User.Tabs.Scores;

public partial class ProfileScore : Container
{
    private APIScore score { get; }

    public ProfileScore(APIScore score)
    {
        this.score = score;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 48;
        CornerRadius = 8;
        Masking = true;

        var rank = Enum.Parse<ScoreRank>(score.Rank);

        var topLine = new FluXisTextFlow { RelativeSizeAxes = Axes.X, AutoSizeAxes = Axes.Y, WebFontSize = 14 };
        topLine.AddText($"{score.Map?.Title} ");
        topLine.AddText<FluXisSpriteText>(score.Map?.Artist, t =>
        {
            t.Alpha = .8f;
            t.WebFontSize = 12;
        });

        var bottomLine = new FluXisTextFlow { RelativeSizeAxes = Axes.X, AutoSizeAxes = Axes.Y, WebFontSize = 12 };
        bottomLine.AddText($"{score.Map?.Difficulty} ");
        bottomLine.AddText<FluXisSpriteText>(TimeUtils.Ago(TimeUtils.GetFromSeconds(score.Time)), t =>
        {
            t.Alpha = .6f;
            t.WebFontSize = 10;
        });

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = DrawableScoreRank.GetColor(rank, true)
            },
            new DrawableScoreRank
            {
                Rank = rank,
                FontSize = FluXisSpriteText.GetWebFontSize(20),
                AlternateColor = true,
                Shadow = false,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.Centre,
                X = Height / 2
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding { Left = 48 },
                Child = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    CornerRadius = 8,
                    Masking = true,
                    Children = new Drawable[]
                    {
                        new LoadWrapper<DrawableOnlineBackground>
                        {
                            RelativeSizeAxes = Axes.Both,
                            OnComplete = d => d.FadeInFromZero(FluXisScreen.FADE_DURATION),
                            LoadContent = () => new DrawableOnlineBackground(score.Map)
                        },
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = FluXisColors.Background2,
                            Alpha = .5f
                        },
                        new GridContainer
                        {
                            RelativeSizeAxes = Axes.Both,
                            ColumnDimensions = new Dimension[]
                            {
                                new(),
                                new(GridSizeMode.AutoSize)
                            },
                            Content = new[]
                            {
                                new Drawable[]
                                {
                                    new FillFlowContainer
                                    {
                                        RelativeSizeAxes = Axes.X,
                                        AutoSizeAxes = Axes.Y,
                                        Anchor = Anchor.CentreLeft,
                                        Origin = Anchor.CentreLeft,
                                        Direction = FillDirection.Vertical,
                                        Margin = new MarginPadding { Horizontal = 12 },
                                        Children = new Drawable[]
                                        {
                                            topLine,
                                            bottomLine
                                        }
                                    },
                                    new FillFlowContainer
                                    {
                                        AutoSizeAxes = Axes.Both,
                                        Anchor = Anchor.CentreRight,
                                        Origin = Anchor.CentreRight,
                                        Direction = FillDirection.Horizontal,
                                        Margin = new MarginPadding { Right = 12 },
                                        Spacing = new Vector2(12),
                                        Children = new Drawable[]
                                        {
                                            new FluXisSpriteText
                                            {
                                                Text = $"{score.PerformanceRating.ToStringInvariant("0.00")}pr",
                                                Anchor = Anchor.CentreRight,
                                                Origin = Anchor.CentreRight,
                                                WebFontSize = 16,
                                                Shadow = true,
                                            },
                                            new FillFlowContainer
                                            {
                                                AutoSizeAxes = Axes.Both,
                                                Anchor = Anchor.CentreRight,
                                                Origin = Anchor.CentreRight,
                                                Direction = FillDirection.Vertical,
                                                Spacing = new Vector2(-2),
                                                Children = new Drawable[]
                                                {
                                                    new FluXisSpriteText
                                                    {
                                                        Text = $"{score.Accuracy.ToStringInvariant("0.00")}%",
                                                        Anchor = Anchor.CentreRight,
                                                        Origin = Anchor.CentreRight,
                                                        WebFontSize = 12,
                                                        Shadow = true,
                                                    },
                                                    new FluXisSpriteText
                                                    {
                                                        Text = $"{score.MaxCombo}x",
                                                        Anchor = Anchor.CentreRight,
                                                        Origin = Anchor.CentreRight,
                                                        WebFontSize = 12,
                                                        Shadow = true,
                                                        Alpha = .8f
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };
    }
}

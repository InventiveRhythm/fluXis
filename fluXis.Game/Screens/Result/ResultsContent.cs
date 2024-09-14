using System;
using fluXis.Game.Screens.Result.Center;
using fluXis.Game.Screens.Result.Sides;
using fluXis.Game.Screens.Result.Sides.Types;
using fluXis.Game.Skinning;
using fluXis.Shared.Scoring;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Utils;

namespace fluXis.Game.Screens.Result;

public partial class ResultsContent : CompositeDrawable
{
    [Resolved]
    private Results screen { get; set; }

    [Resolved]
    private SkinManager skins { get; set; }

    [Resolved]
    private ScoreInfo score { get; set; }

    private Drawable[] rightContent { get; }

    private bool rankMoveSmoothly;
    private bool rankUseCenter;

    private Drawable rank;
    private ResultsHeader header;
    private ResultsSideList left;
    private ResultsCenter center;
    private ResultsSideList right;

    public ResultsContent(Drawable[] rightContent)
    {
        this.rightContent = rightContent;
    }

    [BackgroundDependencyLoader]
    private void load(SkinManager skins)
    {
        RelativeSizeAxes = Axes.Both;
        Padding = new MarginPadding(20) { Bottom = 100 };

        InternalChildren = new[]
        {
            rank = skins.GetResultsScoreRank(score.Rank).With(d =>
            {
                d.RelativePositionAxes = Axes.X;
                d.X = 0.5f;
                d.Origin = Anchor.Centre;
            }),
            new GridContainer
            {
                RelativeSizeAxes = Axes.Both,
                RowDimensions = new Dimension[]
                {
                    new(GridSizeMode.AutoSize),
                    new()
                },
                Content = new[]
                {
                    new Drawable[]
                    {
                        header = new ResultsHeader()
                    },
                    new Drawable[]
                    {
                        new GridContainer
                        {
                            RelativeSizeAxes = Axes.Both,
                            Padding = new MarginPadding(16),
                            ColumnDimensions = new Dimension[]
                            {
                                new(GridSizeMode.Absolute, 440),
                                new(),
                                new(GridSizeMode.Absolute, 440)
                            },
                            Content = new[]
                            {
                                new Drawable[]
                                {
                                    left = new ResultsSideList
                                    {
                                        Children = new Drawable[]
                                        {
                                            new ResultsSideJudgements(skins, score),
                                            new ResultsSideMore(score)
                                        }
                                    },
                                    center = new ResultsCenter(),
                                    right = new ResultsSideList
                                    {
                                        Children = rightContent
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        ScheduleAfterChildren(() => updateRankPosition(true));
    }

    protected override void Update()
    {
        base.Update();
        updateRankPosition(!rankMoveSmoothly);
    }

    private void updateRankPosition(bool instant)
    {
        var y = ToLocalSpace(rankUseCenter ? ScreenSpaceDrawQuad.Centre : center.ScreenSpaceRankPosition).Y;
        y -= Padding.Top;

        if (rankUseCenter)
            y -= Padding.Bottom; // to account for bottom padding

        if (instant)
        {
            rank.Y = y;
            return;
        }

        rank.Y = (float)Interpolation.Lerp(y, rank.Y, Math.Exp(-0.002 * Time.Elapsed));
    }

    public void Show(bool fromGameplay = false)
    {
        if (!fromGameplay)
        {
            rank.ScaleTo(.9f).ScaleTo(1, FluXisScreen.MOVE_DURATION, Easing.OutQuint);
            left.MoveToX(-200).MoveToX(0, FluXisScreen.MOVE_DURATION, Easing.OutQuint);
            center.Show();
            right.MoveToX(200).MoveToX(0, FluXisScreen.MOVE_DURATION, Easing.OutQuint);
            return;
        }

        rankMoveSmoothly = true;
        rankUseCenter = true;

        header.Hide();
        left.FadeOut();
        center.FadeOut();
        right.FadeOut();

        rank.FadeOut().ScaleTo(1.8f)
            .ScaleTo(1, 2000, Easing.OutQuint).FadeIn(400);

        using (BeginDelayedSequence(2000))
        {
            header.MoveToY(-50).MoveToY(0, FluXisScreen.MOVE_DURATION, Easing.OutQuint).FadeIn(FluXisScreen.FADE_DURATION);
            left.MoveToX(-200).MoveToX(0, FluXisScreen.MOVE_DURATION, Easing.OutQuint).FadeIn(FluXisScreen.FADE_DURATION);
            center.FadeIn(FluXisScreen.FADE_DURATION);
            center.Show();
            right.MoveToX(200).MoveToX(0, FluXisScreen.MOVE_DURATION, Easing.OutQuint).FadeIn(FluXisScreen.FADE_DURATION);
            this.TransformTo(nameof(rankUseCenter), false);
        }
    }

    public override void Hide()
    {
        rank.ScaleTo(.9f, FluXisScreen.MOVE_DURATION, Easing.OutQuint);
        left.MoveToX(-200, FluXisScreen.MOVE_DURATION, Easing.OutQuint);
        center.Hide();
        right.MoveToX(200, FluXisScreen.MOVE_DURATION, Easing.OutQuint);
    }
}

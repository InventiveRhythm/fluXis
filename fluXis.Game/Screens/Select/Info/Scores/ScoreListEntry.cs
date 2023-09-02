using System;
using System.Linq;
using fluXis.Game.Audio;
using fluXis.Game.Database.Maps;
using fluXis.Game.Database.Score;
using fluXis.Game.Graphics.Drawables;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Map;
using fluXis.Game.Overlay.Mouse;
using fluXis.Game.Scoring;
using fluXis.Game.Screens.Result;
using fluXis.Game.Skinning;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Framework.Screens;
using osuTK;

namespace fluXis.Game.Screens.Select.Info.Scores;

public partial class ScoreListEntry : Container, IHasDrawableTooltip
{
    [Resolved]
    private MapStore mapStore { get; set; }

    [Resolved]
    private Storage storage { get; set; }

    [Resolved]
    private SkinManager skinManager { get; set; }

    [Resolved]
    private UISamples samples { get; set; }

    public ScoreList ScoreList { get; set; }

    private readonly RealmScore score;
    private readonly int rank;
    private FluXisSpriteText timeText;
    private Container bannerContainer;
    private Container avatarContainer;

    public ScoreListEntry(RealmScore score, int index = -1)
    {
        this.score = score;
        rank = index;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 60;

        CornerRadius = 10;
        Masking = true;

        Children = new Drawable[]
        {
            bannerContainer = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Colour4.Black,
                        Alpha = 0.25f
                    }
                }
            },
            new GridContainer
            {
                ColumnDimensions = new Dimension[]
                {
                    new(GridSizeMode.Absolute, 60),
                    new(),
                    new(GridSizeMode.Absolute, 155)
                },
                RelativeSizeAxes = Axes.Both,
                Content = new[]
                {
                    new Drawable[]
                    {
                        new FluXisSpriteText
                        {
                            Text = $"#{rank}",
                            FontSize = 26,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre
                        },
                        new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            CornerRadius = 10,
                            Masking = true,
                            Children = new Drawable[]
                            {
                                new Box
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Colour = Colour4.Black,
                                    Alpha = 0.25f
                                },
                                avatarContainer = new Container
                                {
                                    Size = new Vector2(50),
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.CentreLeft,
                                    Margin = new MarginPadding(5),
                                    CornerRadius = 5,
                                    Masking = true
                                },
                                new FluXisSpriteText
                                {
                                    Text = score.Player?.Username ?? "Player",
                                    FontSize = 28,
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.BottomLeft,
                                    Padding = new MarginPadding { Left = 60 },
                                    Y = 5
                                },
                                timeText = new FluXisSpriteText
                                {
                                    Text = TimeUtils.Ago(score.Date),
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.TopLeft,
                                    Padding = new MarginPadding { Left = 60 }
                                },
                                new FluXisSpriteText
                                {
                                    Text = score.Accuracy.ToString("00.00").Replace(",", ".") + "%",
                                    FontSize = 28,
                                    Anchor = Anchor.CentreRight,
                                    Origin = Anchor.BottomRight,
                                    Padding = new MarginPadding { Right = 10 },
                                    Y = 5
                                },
                                new FluXisSpriteText
                                {
                                    Text = $"{score.MaxCombo}x",
                                    Anchor = Anchor.CentreRight,
                                    Origin = Anchor.TopRight,
                                    Padding = new MarginPadding { Right = 10 }
                                }
                            }
                        },
                        new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Padding = new MarginPadding { Horizontal = 10 },
                            Children = new Drawable[]
                            {
                                new FillFlowContainer
                                {
                                    RelativeSizeAxes = Axes.X,
                                    AutoSizeAxes = Axes.Y,
                                    Anchor = Anchor.CentreRight,
                                    Origin = Anchor.CentreRight,
                                    Padding = new MarginPadding { Right = 40 },
                                    Direction = FillDirection.Vertical,
                                    Children = new Drawable[]
                                    {
                                        new FluXisSpriteText
                                        {
                                            Text = score.Score.ToString("0000000"),
                                            FontSize = 22,
                                            FixedWidth = true,
                                            Anchor = Anchor.TopRight,
                                            Origin = Anchor.TopRight
                                        },
                                        new FluXisSpriteText
                                        {
                                            Text = score.Mods.Replace(",", " "),
                                            FontSize = 18,
                                            Anchor = Anchor.TopRight,
                                            Origin = Anchor.TopRight
                                        }
                                    }
                                },
                                new DrawableGrade
                                {
                                    Size = 32,
                                    Grade = Enum.TryParse(score.Grade, out Grade grade) ? grade : Grade.D,
                                    Anchor = Anchor.CentreRight,
                                    Origin = Anchor.CentreRight
                                }
                            }
                        }
                    }
                }
            }
        };

        LoadComponentAsync(new DrawableBanner(score.Player)
        {
            RelativeSizeAxes = Axes.Both,
            Depth = 1,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre
        }, bannerContainer.Add);

        LoadComponentAsync(new DrawableAvatar(score.Player)
        {
            RelativeSizeAxes = Axes.Both
        }, avatarContainer.Add);

        this.MoveToX(100).FadeOut()
            .Then((rank - 1) * 50)
            .MoveToX(0, 500, Easing.OutQuint).FadeIn(400);
    }

    protected override void Update()
    {
        timeText.Text = TimeUtils.Ago(score.Date);
        base.Update();
    }

    protected override bool OnHover(HoverEvent e)
    {
        samples.Hover();
        return true;
    }

    protected override bool OnClick(ClickEvent e)
    {
        if (ScoreList == null) return false;

        samples.Click();

        RealmMap map = mapStore.CurrentMapSet.Maps.FirstOrDefault(m => m.ID == score.MapID);

        if (map == null)
        {
            Logger.Log("RealmMap is null", LoggingTarget.Runtime, LogLevel.Error);
            return false;
        }

        MapInfo mapInfo = map.GetMapInfo();
        if (mapInfo == null) return false;

        Performance performance = new Performance(mapInfo, map.OnlineID, map.Hash);

        // find a better way to do this
        for (int i = 0; i < score.Judgements.Flawless; i++) performance.AddJudgement(Judgement.Flawless);
        for (int i = 0; i < score.Judgements.Perfect; i++) performance.AddJudgement(Judgement.Perfect);
        for (int i = 0; i < score.Judgements.Great; i++) performance.AddJudgement(Judgement.Great);
        for (int i = 0; i < score.Judgements.Alright; i++) performance.AddJudgement(Judgement.Alright);
        for (int i = 0; i < score.Judgements.Okay; i++) performance.AddJudgement(Judgement.Okay);
        for (int i = 0; i < score.Judgements.Miss; i++) performance.AddJudgement(Judgement.Miss);
        for (int i = 0; i < score.MaxCombo; i++) performance.IncCombo();

        ScoreList.MapInfo.Screen.Push(new ResultsScreen(map, mapInfo, performance, score.Player, false, false));

        return true;
    }

    public Drawable GetTooltip()
    {
        var date = score.Date;

        return new FillFlowContainer
        {
            Padding = new MarginPadding(10),
            AutoSizeAxes = Axes.Both,
            Direction = FillDirection.Vertical,
            Spacing = new Vector2(10),
            Children = new Drawable[]
            {
                new FluXisSpriteText
                {
                    Text = $"Played on {date:dd MMMM yyyy} at {date:HH:mm}"
                },
                new FillFlowContainer
                {
                    AutoSizeAxes = Axes.Y,
                    Direction = FillDirection.Full,
                    Spacing = new Vector2(10, 5),
                    Width = 290,
                    Children = new Drawable[]
                    {
                        new FluXisSpriteText
                        {
                            Text = $"FLAWLESS {score.Judgements.Flawless}",
                            Colour = skinManager.CurrentSkin.GetColorForJudgement(Judgement.Flawless)
                        },
                        new FluXisSpriteText
                        {
                            Text = $"PERFECT {score.Judgements.Perfect}",
                            Colour = skinManager.CurrentSkin.GetColorForJudgement(Judgement.Perfect)
                        },
                        new FluXisSpriteText
                        {
                            Text = $"GREAT {score.Judgements.Great}",
                            Colour = skinManager.CurrentSkin.GetColorForJudgement(Judgement.Great)
                        },
                        new FluXisSpriteText
                        {
                            Text = $"ALRIGHT {score.Judgements.Alright}",
                            Colour = skinManager.CurrentSkin.GetColorForJudgement(Judgement.Alright)
                        },
                        new FluXisSpriteText
                        {
                            Text = $"OKAY {score.Judgements.Okay}",
                            Colour = skinManager.CurrentSkin.GetColorForJudgement(Judgement.Okay)
                        },
                        new FluXisSpriteText
                        {
                            Text = $"MISS {score.Judgements.Miss}",
                            Colour = skinManager.CurrentSkin.GetColorForJudgement(Judgement.Miss)
                        }
                    }
                }
            }
        };
    }
}

using System;
using fluXis.Game.Audio;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics.Drawables;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Map;
using fluXis.Game.Online.API.Users;
using fluXis.Game.Overlay.Mouse;
using fluXis.Game.Scoring;
using fluXis.Game.Scoring.Enums;
using fluXis.Game.Screens.Result;
using fluXis.Game.Skinning;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Framework.Logging;
using osu.Framework.Screens;
using osuTK;

namespace fluXis.Game.Screens.Select.Info.Scores;

public partial class ScoreListEntry : Container, IHasDrawableTooltip
{
    [Resolved]
    private SkinManager skinManager { get; set; }

    [Resolved]
    private UISamples samples { get; set; }

    public ScoreList ScoreList { get; set; }
    public ScoreInfo ScoreInfo { get; set; }
    public RealmMap Map { get; set; }
    public APIUserShort Player { get; set; }
    public DateTimeOffset Date { get; set; }
    public int Place { get; set; }

    private FluXisSpriteText timeText;
    private Container bannerContainer;
    private Container avatarContainer;

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
                            Text = $"#{Place}",
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
                                    Text = Player?.Username ?? "Player",
                                    FontSize = 28,
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.BottomLeft,
                                    Padding = new MarginPadding { Left = 60 },
                                    Y = 5
                                },
                                timeText = new FluXisSpriteText
                                {
                                    Text = TimeUtils.Ago(Date),
                                    Anchor = Anchor.CentreLeft,
                                    Origin = Anchor.TopLeft,
                                    Padding = new MarginPadding { Left = 60 }
                                },
                                new FluXisSpriteText
                                {
                                    Text = ScoreInfo.Accuracy.ToString("00.00").Replace(",", ".") + "%",
                                    FontSize = 28,
                                    Anchor = Anchor.CentreRight,
                                    Origin = Anchor.BottomRight,
                                    Padding = new MarginPadding { Right = 10 },
                                    Y = 5
                                },
                                new FluXisSpriteText
                                {
                                    Text = $"{ScoreInfo.MaxCombo}x",
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
                                            Text = ScoreInfo.Score.ToString("0000000"),
                                            FontSize = 22,
                                            FixedWidth = true,
                                            Anchor = Anchor.TopRight,
                                            Origin = Anchor.TopRight
                                        },
                                        new FluXisSpriteText
                                        {
                                            Text = string.Join(' ', ScoreInfo.Mods),
                                            FontSize = 18,
                                            Anchor = Anchor.TopRight,
                                            Origin = Anchor.TopRight
                                        }
                                    }
                                },
                                new DrawableScoreRank
                                {
                                    Size = 32,
                                    Rank = ScoreInfo.Rank,
                                    Anchor = Anchor.CentreRight,
                                    Origin = Anchor.CentreRight
                                }
                            }
                        }
                    }
                }
            }
        };

        LoadComponentAsync(new DrawableBanner(Player)
        {
            RelativeSizeAxes = Axes.Both,
            Depth = 1,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre
        }, bannerContainer.Add);

        LoadComponentAsync(new DrawableAvatar(Player)
        {
            RelativeSizeAxes = Axes.Both
        }, avatarContainer.Add);

        this.MoveToX(100).FadeOut()
            .Then((Place - 1) * 50)
            .MoveToX(0, 500, Easing.OutQuint).FadeIn(400);
    }

    protected override void Update()
    {
        timeText.Text = TimeUtils.Ago(Date);
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

        if (Map == null)
        {
            Logger.Log("RealmMap is null", LoggingTarget.Runtime, LogLevel.Error);
            return false;
        }

        MapInfo mapInfo = Map.GetMapInfo();
        if (mapInfo == null) return false;

        ScoreList.MapInfo.Screen.Push(new ResultsScreen(Map, mapInfo, ScoreInfo, Player, false, false));

        return true;
    }

    public Drawable GetTooltip()
    {
        var date = Date;

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
                            Text = $"FLAWLESS {ScoreInfo.Flawless}",
                            Colour = skinManager.CurrentSkin.GetColorForJudgement(Judgement.Flawless)
                        },
                        new FluXisSpriteText
                        {
                            Text = $"PERFECT {ScoreInfo.Perfect}",
                            Colour = skinManager.CurrentSkin.GetColorForJudgement(Judgement.Perfect)
                        },
                        new FluXisSpriteText
                        {
                            Text = $"GREAT {ScoreInfo.Great}",
                            Colour = skinManager.CurrentSkin.GetColorForJudgement(Judgement.Great)
                        },
                        new FluXisSpriteText
                        {
                            Text = $"ALRIGHT {ScoreInfo.Alright}",
                            Colour = skinManager.CurrentSkin.GetColorForJudgement(Judgement.Alright)
                        },
                        new FluXisSpriteText
                        {
                            Text = $"OKAY {ScoreInfo.Okay}",
                            Colour = skinManager.CurrentSkin.GetColorForJudgement(Judgement.Okay)
                        },
                        new FluXisSpriteText
                        {
                            Text = $"MISS {ScoreInfo.Miss}",
                            Colour = skinManager.CurrentSkin.GetColorForJudgement(Judgement.Miss)
                        }
                    }
                }
            }
        };
    }
}

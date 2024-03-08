using System;
using System.Collections.Generic;
using fluXis.Game.Audio;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics.Drawables;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Menus;
using fluXis.Game.Online.API.Models.Users;
using fluXis.Game.Overlay.Mouse;
using fluXis.Game.Skinning;
using fluXis.Game.Utils;
using fluXis.Shared.Scoring;
using fluXis.Shared.Scoring.Enums;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Game.Screens.Select.Info.Scores;

public partial class ScoreListEntry : Container, IHasDrawableTooltip, IHasContextMenu
{
    [Resolved]
    private SkinManager skinManager { get; set; }

    [Resolved]
    private UISamples samples { get; set; }

    [Resolved]
    private FluXisGameBase game { get; set; }

    public MenuItem[] ContextMenuItems
    {
        get
        {
            var items = new List<MenuItem>();

            if (deleted)
                return items.ToArray();

            items.Add(new FluXisMenuItem("View Details", FontAwesome6.Solid.Info, MenuItemType.Highlighted, viewDetails));

            if (DownloadAction != null)
                items.Add(new FluXisMenuItem("Download Replay", FontAwesome6.Solid.ArrowDown, MenuItemType.Normal, () => DownloadAction.Invoke()));

            if (ReplayAction != null)
                items.Add(new FluXisMenuItem("View Replay", FontAwesome6.Solid.Play, MenuItemType.Highlighted, () => ReplayAction.Invoke()));

            if (DeleteAction != null)
            {
                items.Add(new FluXisMenuItem("Delete", FontAwesome6.Solid.Trash, MenuItemType.Dangerous, () =>
                {
                    DeleteAction.Invoke();
                    deleted = true;
                    deletedContainer.FadeIn(200);
                }));
            }

            return items.ToArray();
        }
    }

    public ScoreInfo ScoreInfo { get; init; }
    public RealmMap Map { get; init; }
    public APIUserShort Player { get; init; }
    public int Place { get; set; }

    public Action ReplayAction { get; init; }
    public Action DownloadAction { get; init; }
    public Action DeleteAction { get; init; }

    private DateTimeOffset date;
    private bool deleted;

    private FluXisSpriteText timeText;
    private Container bannerContainer;
    private Container avatarContainer;
    private Container deletedContainer;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 60;
        CornerRadius = 10;
        Masking = true;

        date = TimeUtils.GetFromSeconds(ScoreInfo.Timestamp);

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
            },
            deletedContainer = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Alpha = 0,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = FluXisColors.Background2
                    },
                    new FluXisSpriteText
                    {
                        Text = "Deleted.",
                        FontSize = 28,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre
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
        timeText.Text = TimeUtils.Ago(date);
        base.Update();
    }

    protected override bool OnHover(HoverEvent e)
    {
        samples.Hover();
        return !deleted;
    }

    protected override bool OnClick(ClickEvent e)
    {
        if (deleted) return true;

        samples.Click();
        viewDetails();

        return true;
    }

    private void viewDetails() => game.PresentScore(Map, ScoreInfo, Player);

    public Drawable GetTooltip()
    {
        if (deleted) return null;

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
                            Colour = skinManager.SkinJson.GetColorForJudgement(Judgement.Flawless)
                        },
                        new FluXisSpriteText
                        {
                            Text = $"PERFECT {ScoreInfo.Perfect}",
                            Colour = skinManager.SkinJson.GetColorForJudgement(Judgement.Perfect)
                        },
                        new FluXisSpriteText
                        {
                            Text = $"GREAT {ScoreInfo.Great}",
                            Colour = skinManager.SkinJson.GetColorForJudgement(Judgement.Great)
                        },
                        new FluXisSpriteText
                        {
                            Text = $"ALRIGHT {ScoreInfo.Alright}",
                            Colour = skinManager.SkinJson.GetColorForJudgement(Judgement.Alright)
                        },
                        new FluXisSpriteText
                        {
                            Text = $"OKAY {ScoreInfo.Okay}",
                            Colour = skinManager.SkinJson.GetColorForJudgement(Judgement.Okay)
                        },
                        new FluXisSpriteText
                        {
                            Text = $"MISS {ScoreInfo.Miss}",
                            Colour = skinManager.SkinJson.GetColorForJudgement(Judgement.Miss)
                        }
                    }
                }
            }
        };
    }
}

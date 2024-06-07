using System;
using System.Collections.Generic;
using fluXis.Game.Audio;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics.Drawables;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Menus;
using fluXis.Game.Overlay.Mouse;
using fluXis.Game.Skinning;
using fluXis.Game.Utils;
using fluXis.Shared.Components.Users;
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

public partial class ScoreListEntry : Container, IHasCustomTooltip<ScoreInfo>, IHasContextMenu
{
    [Resolved]
    private UISamples samples { get; set; }

    [Resolved]
    private FluXisGameBase game { get; set; }

    public MenuItem[] ContextMenuItems
    {
        get
        {
            var items = new List<MenuItem>
            {
                new FluXisMenuItem("View Details", FontAwesome6.Solid.Info, MenuItemType.Highlighted, viewDetails)
            };

            if (DownloadAction != null)
                items.Add(new FluXisMenuItem("Download Replay", FontAwesome6.Solid.ArrowDown, MenuItemType.Normal, () => DownloadAction.Invoke()));

            if (ReplayAction != null)
                items.Add(new FluXisMenuItem("View Replay", FontAwesome6.Solid.Play, MenuItemType.Highlighted, () => ReplayAction.Invoke()));

            if (DeleteAction != null)
            {
                items.Add(new FluXisMenuItem("Delete", FontAwesome6.Solid.Trash, MenuItemType.Dangerous, () =>
                {
                    DeleteAction.Invoke();
                }));
            }

            return items.ToArray();
        }
    }

    public ScoreInfo TooltipContent => ScoreInfo;

    public ScoreInfo ScoreInfo { get; init; }
    public RealmMap Map { get; init; }
    public APIUser Player { get; init; }
    public int Place { get; set; }

    public Action ReplayAction { get; init; }
    public Action DownloadAction { get; init; }
    public Action DeleteAction { get; init; }

    private DateTimeOffset date;

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
        return true;
    }

    protected override bool OnClick(ClickEvent e)
    {
        samples.Click();
        viewDetails();

        return true;
    }

    private void viewDetails() => game.PresentScore(Map, ScoreInfo, Player);

    public ITooltip<ScoreInfo> GetCustomTooltip() => new ScoreListEntryTooltip();

    private partial class ScoreListEntryTooltip : CustomTooltipContainer<ScoreInfo>
    {
        private FluXisSpriteText dateText { get; }
        private FluXisSpriteText flawlessText { get; }
        private FluXisSpriteText perfectText { get; }
        private FluXisSpriteText greatText { get; }
        private FluXisSpriteText alrightText { get; }
        private FluXisSpriteText okayText { get; }
        private FluXisSpriteText missText { get; }

        public ScoreListEntryTooltip()
        {
            CornerRadius = 10;
            Child = new FillFlowContainer
            {
                Padding = new MarginPadding(10),
                AutoSizeAxes = Axes.Both,
                Direction = FillDirection.Vertical,
                Spacing = new Vector2(10),
                Children = new Drawable[]
                {
                    dateText = new FluXisSpriteText(),
                    new FillFlowContainer
                    {
                        AutoSizeAxes = Axes.Y,
                        Direction = FillDirection.Full,
                        Spacing = new Vector2(10, 5),
                        Width = 290,
                        Children = new Drawable[]
                        {
                            flawlessText = new FluXisSpriteText(),
                            perfectText = new FluXisSpriteText(),
                            greatText = new FluXisSpriteText(),
                            alrightText = new FluXisSpriteText(),
                            okayText = new FluXisSpriteText(),
                            missText = new FluXisSpriteText()
                        }
                    }
                }
            };
        }

        [BackgroundDependencyLoader]
        private void load(SkinManager skinManager)
        {
            flawlessText.Colour = skinManager.SkinJson.GetColorForJudgement(Judgement.Flawless);
            perfectText.Colour = skinManager.SkinJson.GetColorForJudgement(Judgement.Perfect);
            greatText.Colour = skinManager.SkinJson.GetColorForJudgement(Judgement.Great);
            alrightText.Colour = skinManager.SkinJson.GetColorForJudgement(Judgement.Alright);
            okayText.Colour = skinManager.SkinJson.GetColorForJudgement(Judgement.Okay);
            missText.Colour = skinManager.SkinJson.GetColorForJudgement(Judgement.Miss);
        }

        public override void SetContent(ScoreInfo score)
        {
            var date = TimeUtils.GetFromSeconds(score.Timestamp);

            dateText.Text = $"Played on {date:dd MMMM yyyy} at {date:HH:mm}";

            flawlessText.Text = $"FLAWLESS {score.Flawless}";
            perfectText.Text = $"PERFECT {score.Perfect}";
            greatText.Text = $"GREAT {score.Great}";
            alrightText.Text = $"ALRIGHT {score.Alright}";
            okayText.Text = $"OKAY {score.Okay}";
            missText.Text = $"MISS {score.Miss}";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Audio;
using fluXis.Database.Maps;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Drawables;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Menus;
using fluXis.Mods;
using fluXis.Online.API.Models.Users;
using fluXis.Online.Drawables;
using fluXis.Online.Fluxel;
using fluXis.Overlay.Mouse;
using fluXis.Overlay.User;
using fluXis.Scoring;
using fluXis.Scoring.Enums;
using fluXis.Skinning;
using fluXis.Utils;
using fluXis.Utils.Extensions;
using Humanizer;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osuTK;

namespace fluXis.Screens.Select.Info.Scores;

public partial class ScoreListEntry : Container, IHasCustomTooltip<ScoreInfo>, IHasContextMenu
{
    [Resolved]
    private UISamples samples { get; set; }

    [Resolved]
    private FluXisGameBase game { get; set; }

    [Resolved]
    private IAPIClient api { get; set; }

    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private UserProfileOverlay profileOverlay { get; set; }

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

            if (profileOverlay != null)
                items.Add(new FluXisMenuItem("View Profile", FontAwesome6.Solid.User, MenuItemType.Normal, () => profileOverlay.ShowUser(Player.ID)));

            if (DeleteAction != null)
                items.Add(new FluXisMenuItem("Delete", FontAwesome6.Solid.Trash, MenuItemType.Dangerous, () => DeleteAction.Invoke()));

            return items.ToArray();
        }
    }

    public ScoreInfo TooltipContent => ScoreInfo;

    public ScoreInfo ScoreInfo { get; init; }
    public RealmMap Map { get; init; }
    public APIUser Player { get; init; }
    public int Place { get; set; }
    public bool ShowSelfOutline { get; init; } = true;

    public Action ReplayAction { get; init; }
    public Action DownloadAction { get; init; }
    public Action DeleteAction { get; init; }

    private DateTimeOffset date;

    private Container wrapper;
    private Box rankBackground;
    private FluXisSpriteText timeText;

    private ColourInfo outlineColor
    {
        get
        {
            var color = Colour4.Transparent;

            if (Player.ID == api.User.Value?.ID && ShowSelfOutline)
                color = Colour4.FromHex("#55ff55");
            else if (Player.Following ?? false)
                color = Colour4.Plum;

            return ColourInfo.GradientHorizontal(color, color.Opacity(0));
        }
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 60;

        date = TimeUtils.GetFromSeconds(ScoreInfo.Timestamp);

        Child = wrapper = new Container
        {
            RelativeSizeAxes = Axes.Both,
            CornerRadius = 10,
            Masking = true,
            Children = new Drawable[]
            {
                rankBackground = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = DrawableScoreRank.GetColor(ScoreInfo.Rank, true)
                },
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding { Right = 60 },
                    Child = new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        CornerRadius = 10,
                        Masking = true,
                        Children = new Drawable[]
                        {
                            new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Colour = FluXisColors.Background2
                            },
                            new LoadWrapper<DrawableBanner>
                            {
                                RelativeSizeAxes = Axes.Both,
                                LoadContent = () => new DrawableBanner(Player)
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre
                                },
                                OnComplete = banner => banner.FadeInFromZero(400)
                            },
                            new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Colour = FluXisColors.Background2,
                                Alpha = .6f
                            },
                            new Container
                            {
                                RelativeSizeAxes = Axes.Both,
                                Width = .3f,
                                CornerRadius = 10,
                                Masking = true,
                                BorderColour = outlineColor,
                                BorderThickness = 3,
                                Child = new Box
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    AlwaysPresent = true,
                                    Alpha = 0
                                }
                            },
                            new GridContainer
                            {
                                ColumnDimensions = new Dimension[]
                                {
                                    new(GridSizeMode.Absolute, 60),
                                    new(GridSizeMode.Absolute, 60),
                                    new(),
                                    new(GridSizeMode.AutoSize)
                                },
                                RelativeSizeAxes = Axes.Both,
                                Content = new[]
                                {
                                    new Drawable[]
                                    {
                                        new FluXisSpriteText
                                        {
                                            Text = Place.ToMetric(decimals: 1).Replace(",", "."),
                                            FontSize = 26,
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre
                                        },
                                        new LoadWrapper<DrawableAvatar>
                                        {
                                            RelativeSizeAxes = Axes.Both,
                                            CornerRadius = 10,
                                            Masking = true,
                                            LoadContent = () => new DrawableAvatar(Player)
                                            {
                                                RelativeSizeAxes = Axes.Both,
                                                Anchor = Anchor.Centre,
                                                Origin = Anchor.Centre
                                            },
                                            OnComplete = avatar => avatar.FadeInFromZero(400)
                                        },
                                        new FillFlowContainer
                                        {
                                            RelativeSizeAxes = Axes.Both,
                                            Padding = new MarginPadding { Horizontal = 10 },
                                            Direction = FillDirection.Vertical,
                                            Spacing = new Vector2(8),
                                            Children = new Drawable[]
                                            {
                                                new FillFlowContainer
                                                {
                                                    RelativeSizeAxes = Axes.X,
                                                    Height = 15,
                                                    Direction = FillDirection.Horizontal,
                                                    Anchor = Anchor.CentreLeft,
                                                    Origin = Anchor.CentreLeft,
                                                    Spacing = new Vector2(6),
                                                    Children = new Drawable[]
                                                    {
                                                        new DrawableCountry(Player.GetCountry())
                                                        {
                                                            Width = 20,
                                                            CornerRadius = 4,
                                                            Anchor = Anchor.CentreLeft,
                                                            Origin = Anchor.CentreLeft
                                                        },
                                                        new ClubTag(Player?.Club)
                                                        {
                                                            Alpha = Player?.Club != null ? 1 : 0,
                                                            WebFontSize = 14,
                                                            Anchor = Anchor.CentreLeft,
                                                            Origin = Anchor.CentreLeft
                                                        },
                                                        new FluXisSpriteText
                                                        {
                                                            Text = Player?.PreferredName ?? "Player",
                                                            WebFontSize = 20,
                                                            Anchor = Anchor.CentreLeft,
                                                            Origin = Anchor.CentreLeft
                                                        },
                                                        new FillFlowContainer
                                                        {
                                                            AutoSizeAxes = Axes.Both,
                                                            Anchor = Anchor.CentreLeft,
                                                            Origin = Anchor.CentreLeft,
                                                            Padding = new MarginPadding { Left = 4 },
                                                            Spacing = new Vector2(4),
                                                            Alpha = .8f,
                                                            Children = new Drawable[]
                                                            {
                                                                new FluXisSpriteIcon
                                                                {
                                                                    Size = new Vector2(12),
                                                                    Icon = FontAwesome6.Regular.Clock,
                                                                    Anchor = Anchor.CentreLeft,
                                                                    Origin = Anchor.CentreLeft
                                                                },
                                                                timeText = new FluXisSpriteText
                                                                {
                                                                    WebFontSize = 12,
                                                                    Anchor = Anchor.CentreLeft,
                                                                    Origin = Anchor.CentreLeft
                                                                }
                                                            }
                                                        }
                                                    }
                                                },
                                                new FillFlowContainer
                                                {
                                                    RelativeSizeAxes = Axes.X,
                                                    Height = 15,
                                                    Direction = FillDirection.Horizontal,
                                                    Anchor = Anchor.CentreLeft,
                                                    Origin = Anchor.CentreLeft,
                                                    Spacing = new Vector2(6),
                                                    ChildrenEnumerable = getMods().Concat(new[]
                                                    {
                                                        new FluXisSpriteText
                                                        {
                                                            Text = ScoreInfo.Accuracy.ToString("00.00").Replace(",", ".") + "%",
                                                            WebFontSize = 12,
                                                            Anchor = Anchor.CentreLeft,
                                                            Origin = Anchor.CentreLeft
                                                        },
                                                        new FluXisSpriteText
                                                        {
                                                            Text = $"{ScoreInfo.MaxCombo}x",
                                                            WebFontSize = 12,
                                                            Anchor = Anchor.CentreLeft,
                                                            Origin = Anchor.CentreLeft,
                                                            Alpha = .8f
                                                        }
                                                    })
                                                },
                                            }
                                        },
                                        new FillFlowContainer
                                        {
                                            AutoSizeAxes = Axes.Both,
                                            Anchor = Anchor.CentreRight,
                                            Origin = Anchor.CentreRight,
                                            Direction = FillDirection.Vertical,
                                            Padding = new MarginPadding { Horizontal = 20 },
                                            Spacing = new Vector2(-4),
                                            Children = new Drawable[]
                                            {
                                                new FluXisSpriteText
                                                {
                                                    Text = $"{ScoreInfo.PerformanceRating.ToStringInvariant("00.00")}pr",
                                                    WebFontSize = 20,
                                                    Anchor = Anchor.CentreRight,
                                                    Origin = Anchor.CentreRight
                                                },
                                                new FluXisSpriteText
                                                {
                                                    Text = ScoreInfo.Score.ToString("0000000"),
                                                    WebFontSize = 14,
                                                    Anchor = Anchor.CentreRight,
                                                    Origin = Anchor.CentreRight,
                                                    Alpha = .8f
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
                new Container
                {
                    Size = new Vector2(60),
                    Anchor = Anchor.CentreRight,
                    Origin = Anchor.CentreRight,
                    Child = new DrawableScoreRank
                    {
                        Rank = ScoreInfo.Rank,
                        FontSize = FluXisSpriteText.GetWebFontSize(28),
                        Shadow = false,
                        AlternateColor = true,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre
                    }
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        wrapper.MoveToX(100).FadeOut()
               .Then((Place - 1) * 50)
               .MoveToX(0, 500, Easing.OutQuint).FadeIn(400);

        if (ScoreInfo.Rank == ScoreRank.X)
            rankBackground.Rainbow();
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

    private IEnumerable<Drawable> getMods()
    {
        const float shear = 0.2f;

        var mods = ScoreInfo.Mods;
        mods.Sort();

        foreach (var modstr in mods)
        {
            var mod = ModUtils.GetFromAcronym(modstr);

            if (mod == null)
                continue;

            var rate = mod as RateMod;

            yield return new Container
            {
                Width = rate != null ? 48 : 28,
                Height = 20,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                CornerRadius = 5,
                Masking = true,
                Shear = new Vector2(shear, 0),
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = FluXisColors.GetModTypeColor(mod.Type)
                    },
                    new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        Direction = FillDirection.Horizontal,
                        Children = new Drawable[]
                        {
                            new FluXisSpriteIcon
                            {
                                Size = new Vector2(12),
                                Icon = mod.Icon,
                                Colour = Colour4.Black,
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Shear = new Vector2(-shear, 0),
                                Alpha = .75f
                            },
                            new FluXisSpriteText
                            {
                                WebFontSize = 10,
                                Text = mod.Acronym,
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Colour = Colour4.Black,
                                Shear = new Vector2(-shear, 0),
                                Alpha = rate != null ? .75f : 0
                            }
                        }
                    }
                }
            };
        }
    }

    private void viewDetails() => game.PresentScore(Map, ScoreInfo, Player, ReplayAction);

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

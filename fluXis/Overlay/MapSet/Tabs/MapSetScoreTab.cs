using System;
using System.Linq;
using System.Threading;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Drawables;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Tabs;
using fluXis.Graphics.UserInterface.Text;
using fluXis.Mods.Drawables;
using fluXis.Online.API.Models.Maps;
using fluXis.Online.API.Models.Users;
using fluXis.Online.API.Requests.Maps;
using fluXis.Online.Drawables;
using fluXis.Online.Fluxel;
using fluXis.Scoring.Enums;
using fluXis.Screens.Select.Info.Scores;
using fluXis.Utils;
using fluXis.Utils.Extensions;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions;
using osu.Framework.Extensions.LocalisationExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Overlay.MapSet.Tabs;

public partial class MapSetScoreTab : TabContainer
{
    public override IconUsage Icon => FontAwesome6.Solid.ArrowTrendUp;
    public override string Title => "Scores";

    [Resolved]
    private IAPIClient api { get; set; }

    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private FluXisGame game { get; set; }

    private Bindable<APIMap> bind { get; }

    [CanBeNull]
    private CancellationTokenSource tokenSource;

    private ScoreTable table;

    public MapSetScoreTab(Bindable<APIMap> bind)
    {
        this.bind = bind;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChild = table = new ScoreTable
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            RowSize = new Dimension(GridSizeMode.Absolute, 32),
            Columns = new TableColumn[]
            {
                new("Rank", Anchor.CentreRight, new Dimension(GridSizeMode.Absolute, 36)),
                new("", Anchor.Centre, new Dimension(GridSizeMode.Absolute, 40)),
                new("Player", Anchor.CentreLeft),
                new("", Anchor.CentreLeft, new Dimension(GridSizeMode.Absolute, 40)),
                new("Accuracy", Anchor.CentreLeft, new Dimension(GridSizeMode.Absolute, 72)),
                new("Max Combo", Anchor.CentreLeft, new Dimension(GridSizeMode.Absolute, 80)),
                new("PR", Anchor.CentreLeft, new Dimension(GridSizeMode.Absolute, 44)),
                new("Time", Anchor.CentreLeft, new Dimension(GridSizeMode.Absolute, 48)),
                new("Mods", Anchor.CentreLeft, new Dimension(GridSizeMode.AutoSize, minSize: 112))
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        bind.BindValueChanged(v =>
        {
            tokenSource?.Cancel();
            tokenSource = new CancellationTokenSource();
            changeMap(v.NewValue, tokenSource.Token);
        }, true);
    }

    private void changeMap(APIMap map, CancellationToken cancel)
    {
        var req = new MapLeaderboardRequest(ScoreListType.Global, map.ID);
        req.Success += res =>
        {
            if (cancel.IsCancellationRequested)
                return;

            table.Content = res.Data.Scores.Select((s, idx) => new Drawable[]
            {
                new FluXisSpriteText
                {
                    Text = $"#{idx + 1}",
                    WebFontSize = 12
                },
                new LoadWrapper<DrawableAvatar>
                {
                    Size = new Vector2(24),
                    CornerRadius = 4,
                    Masking = true,
                    LoadContent = () => new DrawableAvatar(s.User)
                    {
                        RelativeSizeAxes = Axes.Both,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre
                    }
                },
                new ClickableContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Action = () => game?.PresentUser(s.User.ID),
                    Child = new FillFlowContainer
                    {
                        AutoSizeAxes = Axes.Both,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Spacing = new Vector2(4),
                        Children = new[]
                        {
                            new ClubTag(s.User.Club)
                            {
                                Alpha = s.User.Club == null ? 0 : 1,
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                                WebFontSize = 10
                            },
                            createName(s.User)
                        }
                    }
                },
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Child = new DrawableScoreRank
                    {
                        FontSize = FluXisSpriteText.GetWebFontSize(14),
                        Rank = Enum.Parse<ScoreRank>(s.Rank),
                        LetterSpacing = 8,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre
                    }
                },
                new FluXisSpriteText
                {
                    Text = s.Accuracy.ToLocalisableString("00.00\\%"),
                    WebFontSize = 12
                },
                new FluXisSpriteText
                {
                    Text = s.MaxCombo.ToLocalisableString("0\\x"),
                    WebFontSize = 12
                },
                new FluXisSpriteText
                {
                    Text = s.PerformanceRating.ToLocalisableString("00"),
                    WebFontSize = 12
                },
                new FluXisSpriteText
                {
                    Text = TimeUtils.AgoShort(TimeUtils.GetFromSeconds(s.Time)),
                    WebFontSize = 12
                },
                new FillFlowContainer
                {
                    AutoSizeAxes = Axes.X,
                    RelativeSizeAxes = Axes.Y,
                    Direction = FillDirection.Horizontal,
                    ChildrenEnumerable = s.Mods.Split(",", StringSplitOptions.RemoveEmptyEntries).Order()
                                          .Select(ModUtils.GetFromAcronym).Where(x => x != null)
                                          .Select(m => new ModIcon
                                          {
                                              Scale = new Vector2(.5f),
                                              Anchor = Anchor.CentreLeft,
                                              Origin = Anchor.CentreLeft,
                                              Mod = m
                                          })
                }
            }).ToArray().ToRectangular();
        };
        api.PerformRequestAsync(req);

        Drawable createName(APIUser user)
        {
            if (user.NamePaint is null)
            {
                return new FluXisSpriteText
                {
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Text = user.Username,
                    WebFontSize = 12
                };
            }

            return new GradientText
            {
                Colour = user.NamePaint.Colors.CreateColorInfo(),
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Text = user.Username,
                WebFontSize = 12
            };
        }
    }

    private partial class ScoreTable : TableContainer
    {
        protected override Drawable CreateHeader(int index, TableColumn column) => new FluXisSpriteText
        {
            Text = column?.Header ?? string.Empty,
            WebFontSize = 10,
            Alpha = .6f
        };
    }
}

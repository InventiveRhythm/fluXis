using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using fluXis.Audio;
using fluXis.Configuration;
using fluXis.Database;
using fluXis.Database.Maps;
using fluXis.Graphics.Containers;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Interaction;
using fluXis.IO;
using fluXis.Localization;
using fluXis.Map;
using fluXis.Online;
using fluXis.Online.API;
using fluXis.Online.API.Models.Users;
using fluXis.Online.API.Requests.Maps;
using fluXis.Online.Fluxel;
using fluXis.Scoring;
using fluXis.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Screens.Select.Info.Tabs.Scores;

public partial class ScoreListTab : SelectInfoTab
{
    public override LocalisableString Title => LocalizationStrings.SongSelect.LeaderboardTitle;
    public override IconUsage Icon => FontAwesome6.Solid.RankingStar;

    [Resolved]
    private MapStore maps { get; set; }

    [Resolved]
    private ScoreManager scoreManager { get; set; }

    [Resolved]
    private UserCache users { get; set; }

    [Resolved]
    private IAPIClient api { get; set; }

    [Resolved]
    private ReplayStorage replays { get; set; }

    [Resolved(CanBeNull = true)]
    private FluXisRealm realm { get; set; }

    [Resolved(CanBeNull = true)]
    private SelectScreen screen { get; set; }

    private readonly List<ScoreListEntry> currentEntries = new();
    public IEnumerable<ScoreInfo> CurrentScores => currentEntries.Select(c => c.ScoreInfo);

    private FluXisScrollContainer list;
    private LoadingIcon loading;

    private FluXisSpriteText noScores;
    private FillFlowContainer outOfDate;

    private RealmMap map;
    private readonly Bindable<ScoreListType> type = new();
    private readonly Bindable<string> username = new();

    private CancellationTokenSource cancelSource;
    private CancellationToken cancel;

    private LeaderboardTypeButton localButton;

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config)
    {
        RelativeSizeAxes = Axes.Both;
        Padding = new MarginPadding(8) { Bottom = 0 };

        type.Value = config.Get<ScoreListType>(FluXisSetting.LeaderboardType);
        username.Value = config.Get<string>(FluXisSetting.Username);

        InternalChildren = new Drawable[]
        {
            noScores = new FluXisSpriteText
            {
                Text = LocalizationStrings.SongSelect.LeaderboardNoScores,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                WebFontSize = 20,
                Alpha = 0
            },
            list = new FluXisScrollContainer
            {
                RelativeSizeAxes = Axes.Both,
                ScrollbarVisible = false,
                Padding = new MarginPadding { Bottom = 52 } // to prevent clipping with footer
            },
            loading = new LoadingIcon
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(32),
                Alpha = 0
            },
            outOfDate = new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Direction = FillDirection.Horizontal,
                Spacing = new Vector2(5),
                Margin = new MarginPadding { Top = 8 },
                Alpha = 0,
                Children = new Drawable[]
                {
                    new FluXisSpriteIcon
                    {
                        Icon = FontAwesome6.Solid.TriangleExclamation,
                        Colour = Theme.Yellow,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Size = new Vector2(20),
                        Shadow = true
                    },
                    new FluXisSpriteText
                    {
                        Text = LocalizationStrings.SongSelect.LeaderboardOutOfDate,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        WebFontSize = 16,
                        Shadow = true
                    }
                }
            }
        };
    }

    public override Drawable CreateHeader()
    {
        var types = Enum.GetValues<ScoreListType>();

        if (!api.CanUseOnline)
        {
            types = new[] { ScoreListType.Local };
            type.Value = ScoreListType.Local;
        }

        var buttons = types.Select(x => new LeaderboardTypeButton(x, type)).ToArray();
        localButton = buttons.First();

        return new FillFlowContainer()
        {
            AutoSizeAxes = Axes.Both,
            Direction = FillDirection.Horizontal,
            ChildrenEnumerable = buttons,
            Padding = new MarginPadding(8)
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        maps.MapBindable.BindValueChanged(onMapChanged, true);
        type.BindValueChanged(_ => Refresh());
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        maps.MapBindable.ValueChanged -= onMapChanged;
    }

    public void Refresh()
    {
        if (map == null)
            return;

        updateMap(map);
    }

    private void onMapChanged(ValueChangedEvent<RealmMap> e) => updateMap(e.NewValue);

    private void updateMap(RealmMap map)
    {
        if (!IsLoaded)
        {
            Schedule(() => updateMap(map));
            return;
        }

        this.map = map;

        loading.Show();
        outOfDate.FadeOut(300);

        cancelSource?.Cancel();
        cancelSource = new CancellationTokenSource();

        list.ScrollToStart();

        foreach (var drawable in list.ScrollContent)
        {
            if (drawable is ScoreListEntry entry)
            {
                if (!entry.Disappearing)
                    drawable.Hide();
            }
            else
                drawable.Expire();
        }

        currentEntries.Clear();
        cancel = cancelSource.Token;
        Task.Run(() => loadScores(cancel), cancel);
    }

    private void loadScores(CancellationToken cancelToken)
    {
        var scores = new List<ScoreListEntry>();

        switch (type.Value)
        {
            case ScoreListType.Local:
                scores.AddRange(scoreManager.OnMap(map.ID).Select(x =>
                {
                    var info = x.ToScoreInfo();
                    var detach = x.Detach();

                    var player = users.Get(info.PlayerID);

                    if (player is null)
                    {
                        var u = APIUser.Default;

                        if (!string.IsNullOrEmpty(username.Value))
                            u.Username = username.Value;

                        player = u;
                    }

                    return new ScoreListEntry
                    {
                        ScoreInfo = info,
                        Map = map,
                        Player = player,
                        ShowSelfOutline = false,
                        DeleteAction = () =>
                        {
                            scoreManager.Delete(detach.ID);
                            Refresh();
                        },
                        ReplayAction = replays.Exists(x.ID) ? () => screen?.ViewReplay(map, detach) : null
                    };
                }));
                break;

            default:
                if (map.OnlineID == -1)
                    replaceNoScores(LocalizationStrings.SongSelect.LeaderboardNotUploaded);
                else if (tryFetchScores(type.Value, cancelToken, out var s))
                    scores.AddRange(s);
                else
                    return;

                break;
        }

        Schedule(() =>
        {
            if (cancelToken.IsCancellationRequested)
                return;

            scores.Sort((a, b) =>
            {
                var res = b.ScoreInfo.PerformanceRating.CompareTo(a.ScoreInfo.PerformanceRating);
                return res == 0 ? b.ScoreInfo.Score.CompareTo(a.ScoreInfo.Score) : res;
            });

            currentEntries.Clear();
            currentEntries.AddRange(scores);

            scores.ForEach(s => addScore(s, scores.IndexOf(s)));
            list.ScrollContent.Add(Empty().With(x =>
            {
                var last = list.ScrollContent.LastOrDefault();

                x.Y = last is null ? 0 : last.Y + last.Height;
                x.RelativeSizeAxes = Axes.X;
                x.Height = 28;
            }));

            var empty = scores.Count == 0;

            if (empty)
            {
                noScores.Text = map.MapSet.AutoImported
                    ? LocalizationStrings.SongSelect.LeaderboardScoresUnavailable
                    : LocalizationStrings.SongSelect.LeaderboardNoScores;
            }

            noScores.FadeTo(empty ? 1 : 0, 300);
            loading.Hide();
        });

        return;

        void addScore(ScoreListEntry entry, int index)
        {
            entry.Place = index + 1;
            LoadComponent(entry);

            entry.Y = (entry.Height + 6) * index;
            list.ScrollContent.Add(entry);
        }
    }

    private bool tryFetchScores(ScoreListType type, CancellationToken cancelToken, out List<ScoreListEntry> scores)
    {
        scores = new List<ScoreListEntry>();

        if (!api.IsLoggedIn)
        {
            replaceNoScores(LocalizationStrings.General.LoginToUse);
            return false;
        }

        var req = new MapLeaderboardRequest(type, map.OnlineID);
        api.PerformRequest(req);

        if (cancelToken.IsCancellationRequested)
            return false;

        if (!req.IsSuccessful)
        {
            replaceNoScores(req.FailReason?.Message ?? APIRequest.UNKNOWN_ERROR);
            return false;
        }

        RealmMap.UpdateLocalInfo(realm, map, req.Response.Data.MapSet, req.Response.Data.Map);

        if (!map.UpToDate)
            Schedule(() => outOfDate.FadeIn(300));

        scores.AddRange(req.Response.Data.Scores.Select(x => new ScoreListEntry
        {
            ScoreInfo = x.ToScoreInfo(),
            Map = map,
            Player = x.User,
            DownloadAction = () => scoreManager.DownloadScore(map, x),
            DownloadFinishedAction = d =>
            {
                var ssp = d.ScreenSpaceDrawQuad.Centre;
                var pad = new Vector2(Padding.Left, Padding.Top);
                var pos = ToLocalSpace(ssp) - pad;

                var circle = new Circle
                {
                    Size = d.Size,
                    Colour = d.Colour,
                    Anchor = Anchor.TopLeft,
                    Origin = Anchor.Centre,
                    Position = pos
                };

                AddInternal(circle);

                var target = ToLocalSpace(localButton.ScreenSpaceDrawQuad.Centre) - pad;
                circle.MoveTo(target, 600, Easing.OutQuint).ScaleTo(0f, 600, Easing.OutQuint).Expire();
            }
        }));

        return true;
    }

    private void replaceNoScores(LocalisableString text) => Scheduler.ScheduleIfNeeded(() =>
    {
        noScores.Text = text;
        noScores.FadeIn(300);
        loading.Show();
    });

    private partial class LeaderboardTypeButton : CompositeDrawable
    {
        [Resolved]
        private UISamples samples { get; set; }

        private Container content;
        private HoverLayer hover;
        private FlashLayer flash;

        private ScoreListType type { get; }
        private Bindable<ScoreListType> current { get; }

        public LeaderboardTypeButton(ScoreListType type, Bindable<ScoreListType> current)
        {
            this.type = type;
            this.current = current;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Width = 96;
            Height = 32;
            Shear = new Vector2(.1f, 0);

            var color = type switch
            {
                ScoreListType.Local => Colour4.FromHSV(120f / 360f, .6f, 1f),
                ScoreListType.Global => Colour4.FromHSV(30f / 360f, .6f, 1f),
                ScoreListType.Country => Colour4.FromHSV(0f, .6f, 1f),
                ScoreListType.Friends => Colour4.FromHSV(210f / 360f, .6f, 1f),
                ScoreListType.Club => Colour4.FromHSV(270f / 360f, .6f, 1f),
                _ => Theme.Background4
            };

            InternalChild = content = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                CornerRadius = 5,
                Masking = true,
                BorderColour = ColourInfo.GradientVertical(color, color.Lighten(1)),
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = color
                    },
                    hover = new HoverLayer(),
                    flash = new FlashLayer(),
                    new TruncatingText
                    {
                        MaxWidth = 80,
                        Text = type switch
                        {
                            ScoreListType.Local => LocalizationStrings.SongSelect.LeaderboardLocal,
                            ScoreListType.Global => LocalizationStrings.SongSelect.LeaderboardGlobal,
                            ScoreListType.Country => LocalizationStrings.SongSelect.LeaderboardCountry,
                            ScoreListType.Friends => LocalizationStrings.SongSelect.LeaderboardFriends,
                            ScoreListType.Club => LocalizationStrings.SongSelect.LeaderboardClub,
                            _ => "???"
                        },
                        WebFontSize = 12,
                        Shear = new Vector2(-.1f, 0),
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Colour = Colour4.Black,
                        Alpha = .75f
                    }
                }
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            current.BindValueChanged(v => content.BorderThickness = v.NewValue == type ? 3 : 0, true);
        }

        protected override bool OnClick(ClickEvent e)
        {
            flash.Show();
            samples.Click();
            current.Value = type;
            return true;
        }

        protected override bool OnHover(HoverEvent e)
        {
            hover.Show();
            samples.Hover();
            return true;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            hover.Hide();
        }

        protected override bool OnMouseDown(MouseDownEvent e)
        {
            content.ScaleTo(.9f, 1000, Easing.OutQuint);
            return true;
        }

        protected override void OnMouseUp(MouseUpEvent e)
        {
            content.ScaleTo(1, 1000, Easing.OutElastic);
        }
    }
}

public enum ScoreListType
{
    Local,
    Global,
    Country,
    Friends,
    Club
}

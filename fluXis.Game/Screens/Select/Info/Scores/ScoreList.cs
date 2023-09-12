using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using fluXis.Game.Database;
using fluXis.Game.Database.Maps;
using fluXis.Game.Database.Score;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Context;
using fluXis.Game.Online;
using fluXis.Game.Online.API;
using fluXis.Game.Online.API.Scores;
using fluXis.Game.Online.Fluxel;
using Newtonsoft.Json;
using osu.Framework.Allocation;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Game.Screens.Select.Info.Scores;

public partial class ScoreList : GridContainer
{
    [Resolved]
    private FluXisRealm realm { get; set; }

    [Resolved]
    private Fluxel fluxel { get; set; }

    public SelectMapInfo MapInfo { get; init; }

    private RealmMap map;
    private ScoreListType type = ScoreListType.Local;

    private CancellationTokenSource cancellationTokenSource;
    private CancellationToken cancellationToken;

    private FluXisSpriteText noScoresText;
    private FluXisScrollContainer scrollContainer;
    private FillFlowContainer<ClickableText> typeSwitcher;
    private LoadingIcon loadingIcon;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        RowDimensions = new[]
        {
            new Dimension(GridSizeMode.AutoSize),
            new Dimension()
        };

        Content = new[]
        {
            new Drawable[]
            {
                typeSwitcher = new FillFlowContainer<ClickableText>
                {
                    AutoSizeAxes = Axes.Both,
                    Direction = FillDirection.Horizontal,
                    Spacing = new Vector2(40),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Margin = new MarginPadding { Bottom = 10 },
                    Children = new ClickableText[]
                    {
                        new()
                        {
                            ScoreList = this,
                            Type = ScoreListType.Global
                        },
                        new()
                        {
                            ScoreList = this,
                            Type = ScoreListType.Local
                        }
                    }
                }
            },
            new Drawable[]
            {
                new FluXisContextMenuContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Children = new Drawable[]
                    {
                        noScoresText = new FluXisSpriteText
                        {
                            Text = "No scores yet!",
                            FontSize = 32,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Alpha = 0
                        },
                        scrollContainer = new FluXisScrollContainer
                        {
                            RelativeSizeAxes = Axes.Both,
                            ScrollbarAnchor = Anchor.TopRight
                        },
                        loadingIcon = new LoadingIcon
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Size = new Vector2(50),
                            Alpha = 0
                        }
                    }
                }
            }
        };

        updateTabs();
    }

    public void Refresh()
    {
        if (map == null)
            return;

        SetMap(map);
        updateTabs();
    }

    private void updateTabs() => typeSwitcher.Children.ForEach(c => c.Selected = c.Type == type);

    public void SetMap(RealmMap map)
    {
        if (!IsLoaded)
        {
            Schedule(() => SetMap(map));
            return;
        }

        loadingIcon.FadeIn(200);

        cancellationTokenSource?.Cancel();
        cancellationTokenSource = new CancellationTokenSource();

        scrollContainer.ScrollContent.Clear();
        this.map = map;

        cancellationToken = cancellationTokenSource.Token;
        Task.Run(() => loadScores(cancellationToken), cancellationToken);
    }

    private void loadScores(CancellationToken cancellationToken)
    {
        List<ScoreListEntry> scores = new();

        switch (type)
        {
            case ScoreListType.Local:
                realm?.Run(r => r.All<RealmScore>().ToList().ForEach(s =>
                {
                    if (s.MapID == map.ID)
                    {
                        scores.Add(new ScoreListEntry
                        {
                            ScoreInfo = s.ToScoreInfo(),
                            Map = map,
                            Player = s.Player,
                            Date = s.Date,
                            Deletable = true,
                            RealmScoreId = s.ID
                        });
                    }
                }));
                break;

            case ScoreListType.Global:
                if (map.OnlineID == -1)
                {
                    noScoresText.Text = "Scores are not available for this map!";
                    Schedule(() =>
                    {
                        noScoresText.FadeIn(200);
                        loadingIcon.FadeOut(200);
                    });
                    return;
                }

                var request = fluxel.CreateAPIRequest($"/map/{map.OnlineID}/scores", HttpMethod.Get);
                request.Perform();

                var json = request.GetResponseString();
                var rsp = JsonConvert.DeserializeObject<APIResponse<APIScores>>(json);

                if (rsp.Status != 200)
                {
                    noScoresText.Text = "Something went wrong!";
                    Schedule(() => noScoresText.FadeTo(1, 200));
                    return;
                }

                if (map.Status != rsp.Data.Map.Status)
                {
                    map.MapSet.SetStatus(rsp.Data.Map.Status);
                    realm?.RunWrite(r =>
                    {
                        var m = r.Find<RealmMap>(map.ID);
                        m.MapSet.SetStatus(rsp.Data.Map.Status);
                    });
                }

                foreach (var score in rsp.Data.Scores)
                {
                    scores.Add(new ScoreListEntry
                    {
                        ScoreInfo = score.ToScoreInfo(),
                        Map = map,
                        Player = UserCache.GetUser(score.UserId),
                        Date = DateTimeOffset.FromUnixTimeSeconds(score.Time)
                    });
                }

                break;
        }

        Schedule(() =>
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            scores.Sort((a, b) => b.ScoreInfo.Score.CompareTo(a.ScoreInfo.Score));
            scores.ForEach(s => addScore(s, scores.IndexOf(s) + 1));

            if (scrollContainer.ScrollContent.Children.Count == 0)
                noScoresText.Text = map.MapSet.Managed ? "Scores are not available for this map!" : "No scores yet!";

            noScoresText.FadeTo(scrollContainer.ScrollContent.Children.Count == 0 ? 1 : 0, 200);
            loadingIcon.FadeOut(200);
        });
    }

    private void addScore(ScoreListEntry entry, int index = -1)
    {
        entry.ScoreList = this;
        entry.Place = index;
        entry.Y = scrollContainer.ScrollContent.Children.Count > 0 ? scrollContainer.ScrollContent.Children[^1].Y + scrollContainer.ScrollContent.Children[^1].Height + 5 : 0;
        scrollContainer.ScrollContent.Add(entry);
    }

    private partial class ClickableText : ClickableContainer
    {
        public ScoreListType Type { get; init; }
        public ScoreList ScoreList { get; init; }

        public bool Selected
        {
            set => Schedule(() => text.FadeColour(value ? FluXisColors.Text : FluXisColors.Text2, 200));
        }

        private FluXisSpriteText text;

        [BackgroundDependencyLoader]
        private void load()
        {
            AutoSizeAxes = Axes.Both;
            Child = text = new FluXisSpriteText
            {
                Text = Type.ToString(),
                FontSize = 32,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Colour = FluXisColors.Text2,
                Shadow = true
            };

            Action = () =>
            {
                ScoreList.type = Type;
                ScoreList.Refresh();
            };
        }
    }
}

public enum ScoreListType
{
    Local,
    Global
}

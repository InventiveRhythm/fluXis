using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using fluXis.Game.Database;
using fluXis.Game.Database.Maps;
using fluXis.Game.Database.Score;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Online.API;
using fluXis.Game.Online.API.Scores;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Scoring;
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

    public SelectMapInfo MapInfo { get; set; }

    private RealmMap map;
    private ScoreListType type = ScoreListType.Local;

    private FluXisSpriteText noScoresText;
    private FluXisScrollContainer scrollContainer;
    private FillFlowContainer<ClickableText> typeSwitcher;

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
                new Container
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

        scrollContainer.ScrollContent.Clear();
        this.map = map;

        List<RealmScore> scores = new();

        switch (type)
        {
            case ScoreListType.Local:
                realm?.Run(r => r.All<RealmScore>().ToList().ForEach(s =>
                {
                    if (s.MapID == map.ID)
                        scores.Add(s);
                }));
                scores.Sort((s1, s2) => s2.Score.CompareTo(s1.Score));
                break;

            case ScoreListType.Global:
                if (map.OnlineID == -1)
                {
                    noScoresText.Text = "Scores are not available for this map!";
                    noScoresText.FadeTo(1, 200);
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

                rsp.Data.Scores.ForEach(s => scores.Add(new RealmScore(map)
                {
                    ID = default,
                    PlayerID = s.UserId,
                    OnlineID = map.OnlineID,
                    Accuracy = s.Accuracy,
                    Grade = s.Grade,
                    Score = s.TotalScore,
                    MaxCombo = s.MaxCombo,
                    Judgements = new RealmJudgements(new Dictionary<Judgement, int>
                    {
                        { Judgement.Flawless, s.FlawlessCount },
                        { Judgement.Perfect, s.PerfectCount },
                        { Judgement.Great, s.GreatCount },
                        { Judgement.Alright, s.AlrightCount },
                        { Judgement.Okay, s.OkayCount },
                        { Judgement.Miss, s.MissCount }
                    }),
                    Mods = s.Mods,
                    MapID = map.ID,
                    Date = DateTimeOffset.FromUnixTimeSeconds(s.Time)
                }));

                break;
        }

        scores.ForEach(s => addScore(s, scores.IndexOf(s) + 1));

        noScoresText.Text = map.MapSet.Managed ? "Scores are not available for this map!" : "No scores yet!";
        noScoresText.FadeTo(scrollContainer.ScrollContent.Children.Count == 0 ? 1 : 0, 200);
    }

    private void addScore(RealmScore score, int index = -1)
    {
        if (score.MapID != map.ID)
            return;

        var entry = new ScoreListEntry(score, index) { ScoreList = this };
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

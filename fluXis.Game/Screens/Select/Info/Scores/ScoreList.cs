using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Database;
using fluXis.Game.Database.Maps;
using fluXis.Game.Database.Score;
using fluXis.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Screens.Select.Info.Scores;

public partial class ScoreList : Container
{
    [Resolved]
    private FluXisRealm realm { get; set; }

    private RealmMap map;

    private readonly SpriteText noScoresText;
    private readonly BasicScrollContainer scrollContainer;

    public ScoreList()
    {
        RelativeSizeAxes = Axes.Both;

        InternalChildren = new Drawable[]
        {
            noScoresText = new SpriteText
            {
                Text = "No scores yet!",
                Font = FluXisFont.Default(32),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Alpha = 0
            },
            scrollContainer = new BasicScrollContainer
            {
                RelativeSizeAxes = Axes.Both,
                ScrollbarVisible = false
            }
        };
    }

    public void Refresh()
    {
        if (map == null)
            return;

        SetMap(map);
    }

    public void SetMap(RealmMap map)
    {
        scrollContainer.ScrollContent.Clear();
        this.map = map;

        List<RealmScore> scores = new();
        realm.Run(r => r.All<RealmScore>().ToList().ForEach(s =>
        {
            if (s.MapID == map.ID)
                scores.Add(s);
        }));

        scores.Sort((s1, s2) => s2.Score.CompareTo(s1.Score));
        scores.ForEach(s => addScore(s, scores.IndexOf(s) + 1));

        noScoresText.FadeTo(scrollContainer.ScrollContent.Children.Count == 0 ? 1 : 0, 200);
    }

    private void addScore(RealmScore score, int index = -1)
    {
        if (score.MapID != map.ID)
            return;

        var entry = new ScoreListEntry(score, index);
        entry.Y = scrollContainer.ScrollContent.Children.Count > 0 ? scrollContainer.ScrollContent.Children.Last().Y + scrollContainer.ScrollContent.Children.Last().Height + 5 : 0;
        scrollContainer.ScrollContent.Add(entry);
    }
}

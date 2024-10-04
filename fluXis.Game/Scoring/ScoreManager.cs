using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Database;
using fluXis.Game.Database.Maps;
using fluXis.Game.Database.Score;
using fluXis.Game.Map;
using fluXis.Game.Scoring.Processing;
using fluXis.Game.Utils;
using fluXis.Shared.Scoring;
using osu.Framework.Allocation;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics.Containers;
using osu.Framework.Logging;

namespace fluXis.Game.Scoring;

#nullable enable

public partial class ScoreManager : CompositeComponent
{
    public event Action<Guid, RealmScore>? ScoreAdded;
    public event Action<Guid, RealmScore>? ScoreRemoved;
    public event Action<Guid, RealmScore?>? TopScoreUpdated;

    private Dictionary<Guid, Guid> highestScores { get; } = new();

    public const int SCORE_VERSION = 2;

    [Resolved]
    private FluXisRealm realm { get; set; } = null!;

    [Resolved]
    private MapStore maps { get; set; } = null!;

    private bool initialProcessing = true;

    [BackgroundDependencyLoader]
    private void load()
    {
        checkForUpdates();
        maps.MapSets.ForEach(set => set.Maps.ForEach(map => processMap(map.ID)));
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        initialProcessing = false;
    }

    private void checkForUpdates()
    {
        realm.RunWrite(r =>
        {
            var old = r.All<RealmScore>().Where(s => s.Version < SCORE_VERSION);
            var failed = 0;

            foreach (var score in old)
            {
                try
                {
                    for (int i = score.Version + 1; i < SCORE_VERSION + 1; i++)
                    {
                        processUpdate(score, i);
                        score.Version = i;
                    }
                }
                catch
                {
                    failed++;
                }
            }

            if (failed > 0)
                Logger.Log($"Failed to update ${failed} scores!");
        });
    }

    public RealmScore Add(Guid map, ScoreInfo score, long onlineID = -1) => realm.RunWrite(r =>
    {
        var rScore = r.Add(RealmScore.FromScoreInfo(map, score, onlineID)).Detach();

        Schedule(() =>
        {
            ScoreAdded?.Invoke(map, rScore);
            processMap(map);
        });

        return rScore;
    });

    public IEnumerable<RealmScore> OnMap(Guid map) => realm.Run(r =>
    {
        return r.All<RealmScore>()
                .Where(s => s.MapID == map).ToList()
                .Select(s => s.Detach()).ToList();
    });

    public void Delete(Guid id) => realm.RunWrite(r =>
    {
        var score = r.Find<RealmScore>(id);

        if (score == null)
            return;

        var detach = score.Detach();
        r.Remove(score);

        Schedule(() =>
        {
            ScoreRemoved?.Invoke(detach.MapID, detach);
            processMap(detach.MapID);
        });
    });

    public void UpdateOnlineID(Guid score, long id) => realm.RunWrite(r =>
    {
        var rScore = r.Find<RealmScore>(score);

        if (rScore is null)
            return;

        rScore.OnlineID = id;
    });

    public RealmScore? GetCurrentTop(Guid map)
    {
        if (highestScores.TryGetValue(map, out var top))
            return realm.Run(r => r.Find<RealmScore>(top).Detach());

        return null;
    }

    private void processMap(Guid map) => realm.Run(r =>
    {
        var scores = r.All<RealmScore>().Where(s => s.MapID == map).ToList();

        RealmScore? top = null;

        foreach (var score in scores)
        {
            if (top is null)
                top = score;
            else if (top.Score < score.Score)
                top = score;
        }

        if (top is null)
        {
            if (!initialProcessing)
                TopScoreUpdated?.Invoke(map, null);

            return;
        }

        var hasPrevious = highestScores.TryGetValue(map, out var previous);
        highestScores[map] = top.ID;

        if (!initialProcessing && hasPrevious && previous != top.ID)
            TopScoreUpdated?.Invoke(map, top.Detach());
    });

    private void processUpdate(RealmScore score, long version)
    {
        switch (version)
        {
            case 2:
            {
                var map = realm.Run(r => r.Find<RealmMap>(score.MapID).Detach());

                if (map is null)
                    return;

                var mods = score.Mods.Split(' ').Select(ModUtils.GetFromAcronym).ToList();
                score.PerformanceRating = ScoreProcessor.CalculatePerformance(
                    map.Filters.NotesPerSecond,
                    score.Accuracy,
                    score.Flawless,
                    score.Perfect,
                    score.Great,
                    score.Alright,
                    score.Okay,
                    score.Miss,
                    mods
                );
                break;
            }
        }
    }
}

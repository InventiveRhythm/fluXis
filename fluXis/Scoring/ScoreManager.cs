using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using fluXis.Database;
using fluXis.Database.Maps;
using fluXis.Database.Score;
using fluXis.IO;
using fluXis.Map;
using fluXis.Online.API.Models.Scores;
using fluXis.Online.Fluxel;
using fluXis.Replays;
using fluXis.Scoring.Processing;
using fluXis.Utils;
using fluXis.Utils.Downloading;
using fluXis.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics.Containers;
using osu.Framework.IO.Network;
using osu.Framework.Logging;
using Realms;

namespace fluXis.Scoring;

#nullable enable

public partial class ScoreManager : CompositeDrawable
{
    public event Action<Guid, RealmScore>? ScoreAdded;
    public event Action<Guid, RealmScore>? ScoreRemoved;
    public event Action<Guid, RealmScore?>? TopScoreUpdated;

    private Dictionary<Guid, Guid> highestScores { get; } = new();

    public const int SCORE_VERSION = 3;

    [Resolved]
    private FluXisRealm realm { get; set; } = null!;

    [Resolved]
    private ReplayStorage replays { get; set; } = null!;

    [Resolved]
    private MapStore maps { get; set; } = null!;

    [Resolved]
    private IAPIClient api { get; set; } = null!;

    private bool initialProcessing = true;

    [BackgroundDependencyLoader]
    private void load()
    {
        checkForUpdates();
        reprocessAll();
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        initialProcessing = false;

        api.User.ValueChanged += _ => Scheduler.ScheduleIfNeeded(reprocessAll);
    }

    private void reprocessAll() => maps.MapSets.ForEach(set => set.Maps.ForEach(map => processMap(map.ID)));

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
                Logger.Log($"Failed to update {failed} scores!");
        });
    }

    public RealmScore Add(Guid map, ScoreInfo score, long onlineID = -1)
    {
        var rScore = realm.RunWrite(r => r.Add(RealmScore.FromScoreInfo(map, score, onlineID)).Detach());

        // the score isn't instantly available when adding it
        // so the delay is kinda necessary
        Scheduler.AddDelayed(() =>
        {
            ScoreAdded?.Invoke(map, rScore);
            processMap(map);
        }, 100);

        return rScore;
    }

    public IEnumerable<ScorePair> OnMap(Guid map) => realm.Run(r =>
    {
        var realmScores = r.All<RealmScore>()
                           .Where(s => s.MapID == map).ToList()
                           .Select(s => s.Detach()).ToList();

        var scoreInfos = new List<ScorePair>();

        foreach (var realmScore in realmScores)
        {
            scoreInfos.Add(new ScorePair(realmScore.ToScoreInfo(), realmScore));
        }

        return scoreInfos;
    });

    public void Delete(Guid id) => realm.RunWrite(r =>
    {
        var score = r.Find<RealmScore>(id);

        if (score == null)
            return;

        var detach = score.Detach();
        r.Remove(score);

        Scheduler.ScheduleIfNeeded(() =>
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

    public void WipeFromMap(Guid map) => realm.RunWrite(r =>
    {
        var scores = r.All<RealmScore>().Where(s => s.MapID == maps.CurrentMap.ID);
        var detach = scores.ToList().Select(x => x.Detach()).ToList();
        r.RemoveRange(scores);

        Scheduler.ScheduleIfNeeded(() =>
        {
            detach.ForEach(s => ScoreRemoved?.Invoke(map, s));
            processMap(map);
        });
    });

    public RealmScore? GetCurrentTop(Guid map)
    {
        if (highestScores.TryGetValue(map, out var top))
            return realm.Run(r => r.Find<RealmScore>(top).Detach());

        return null;
    }

    public ScoreInfo? GetCurrentTopScoreInfo(Guid map)
    {
        if (highestScores.TryGetValue(map, out var top))
        {
            return realm.Run(r =>
            {
                var result = r.Find<RealmScore>(top).Detach();
                return result?.ToScoreInfo();
            });
        }

        return null;
    }

    private void processMap(Guid map) => realm.Run(r =>
    {
        var player = api.User.Value?.ID ?? 0;

        //unsure about this, hopefully this actually queries all the score the current player
        var scores = r.All<RealmScore>()
                      .Filter("MapID == $0 AND Players.PlayerID == $1", map, player)
                      .ToList();

        RealmScore? top = null;

        //TODO: we might have two top score for dual maps in the future, just assume player 0 for now
        foreach (var score in scores)
        {
            if (top is null)
                top = score;
            else if (top.Players[0].Score < score.Players[0].Score)
                top = score;
        }

        if (top is null)
        {
            if (!initialProcessing)
            {
                TopScoreUpdated?.Invoke(map, null);
                highestScores.Remove(map);
            }

            return;
        }

        highestScores.TryGetValue(map, out var previous);
        highestScores[map] = top.ID;

        if (initialProcessing)
            return;

        if (!initialProcessing && previous != top.ID)
            TopScoreUpdated?.Invoke(map, top.Detach());
    });

    private void processUpdate(RealmScore score, long version)
    {
        switch (version)
        {
            case 2:
            case 3:
            {
                var map = realm.Run(r => r.Find<RealmMap>(score.MapID).Detach());

                if (map is null)
                    return;

                var mods = score.Mods.Split(' ').Select(ModUtils.GetFromAcronym).ToList();

                foreach (var realmPlayerScore in score.Players)
                {
                    //TODO: might need some more work if dual maps have different max PR values for each sides
                    realmPlayerScore.PerformanceRating = ScoreProcessor.CalculatePerformance(
                        map.Rating,
                        realmPlayerScore.Accuracy,
                        realmPlayerScore.Flawless,
                        realmPlayerScore.Perfect,
                        realmPlayerScore.Great,
                        realmPlayerScore.Alright,
                        realmPlayerScore.Okay,
                        realmPlayerScore.Miss,
                        mods
                    );
                }

                break;
            }
        }
    }

    #region Downloading

    private readonly List<DownloadStatus> downloading = new();
    private readonly object downloadLock = new { };

    public DownloadStatus? DownloadScore(RealmMap map, APIScore score)
    {
        if (realm.Run(r => r.All<RealmScore>().Any(x => x.OnlineID == score.ID)))
            return null;

        if (IsDownloading(score.ID, out var existing))
            return existing;

        var status = new DownloadStatus(score.ID);

        lock (downloadLock)
            downloading.Add(status);

        var req = new WebRequest($"{api.Endpoint.AssetUrl}/replay/{score.ID}.frp");
        req.DownloadProgress += (cur, max) => status.Progress = cur / (float)max;
        req.AllowInsecureRequests = true;

        req.Finished += () =>
        {
            try
            {
                var json = req.GetResponseString();
                var replay = json.Deserialize<Replay>();

                var rsc = Add(map.ID, score.ToScoreInfo(), score.ID);
                replays.Save(replay, rsc.ID);

                status.State = DownloadState.Finished;
                finish(status);
            }
            catch (Exception ex)
            {
                fail(ex);
            }
        };

        req.Failed += fail;

        _ = req.PerformAsync();
        return status;

        void fail(Exception ex)
        {
            Logger.Error(ex, "Failed to download score!");
            status.State = DownloadState.Failed;
            finish(status);
        }

        void finish(DownloadStatus s)
        {
            lock (downloadLock)
                downloading.Remove(s);
        }
    }

    public bool IsDownloading(long id, [NotNullWhen(true)] out DownloadStatus? status)
    {
        lock (downloadLock)
        {
            status = downloading.FirstOrDefault(x => x.OnlineID == id);
            return status is not null;
        }
    }

    #endregion
}

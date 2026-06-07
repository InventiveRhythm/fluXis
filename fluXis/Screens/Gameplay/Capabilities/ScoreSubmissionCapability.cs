using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using fluXis.Graphics;
using fluXis.Online.API.Models.Users;
using fluXis.Online.API.Requests.Scores;
using fluXis.Online.Fluxel;
using fluXis.Replays;
using fluXis.Scoring;
using fluXis.Screens.Gameplay.Capabilities.Bases;
using fluXis.Screens.Gameplay.UI;
using fluXis.Screens.Result;
using Midori.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Framework.Screens;

namespace fluXis.Screens.Gameplay.Capabilities;

public partial class ScoreSubmissionCapability : Component, IEndingCapability
{
    public GameplayScreen Screen { get; set; }

    [Resolved]
    private ScoreManager scores { get; set; }

    [Resolved]
    private IAPIClient api { get; set; }

    [Resolved]
    private Storage storage { get; set; }

    private ScoreSubmissionOverlay overlay;

    public void PostLoad()
    {
        Screen.Add(overlay = new ScoreSubmissionOverlay());
    }

    public Screen OnEnd(ScoreInfo score, Action complete)
    {
        var screen = new SoloResults(Screen.RealmMap, score, Screen.PlayfieldManager.Players[0].ScoreProcessor.Player ?? APIUser.Default);
        var canBeUploaded = Screen.Mods.All(m => m.Rankable) && Screen.RealmMap.StatusInt < 100 && !Screen.RealmMap.MapSet.AutoImported;

        if (canBeUploaded)
            overlay.FadeIn(Styling.TRANSITION_FADE);

        screen.OnRestart = Screen.OnRestart;

        var bestScore = scores.GetCurrentTop(Screen.RealmMap.ID);
        if (bestScore != null) screen.ComparisonScore = bestScore.ToScoreInfo();

        Task.Run(() =>
        {
            if (Screen.Mods.All(m => m.SaveScore) && !Screen.RealmMap.MapSet.AutoImported)
            {
                var id = scores.Add(Screen.RealmMap.ID, score).ID;
                var replay = saveReplay(id);

                if (canBeUploaded)
                {
                    var request = new ScoreSubmitRequest(score, Screen.Mods, replay, Screen.Map.Hash, Screen.Map.EffectHash, Screen.Map.StoryboardHash);
                    screen.SubmitRequest = request;
                    api.PerformRequest(request);

                    var resData = request.Response?.Data;

                    if (request.IsSuccessful && resData?.Score != null)
                        scores.UpdateOnlineID(id, resData.Score.ID);

                    Schedule(() =>
                    {
                        overlay.FadeOut(Styling.TRANSITION_FADE);
                        complete();
                    });
                }
            }
        });

        return screen;
    }

    private Replay saveReplay(Guid scoreID)
    {
        try
        {
            var replay = Screen.ReplayRecorder.Replay;
            replay.PlayerID = api.User.Value?.ID ?? -1;
            var folder = storage.GetFullPath("replays");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var path = Path.Combine(folder, $"{scoreID}.frp");
            File.WriteAllText(path, replay.Serialize());

            return replay;
        }
        catch (Exception e)
        {
            Logger.Error(e, "Failed to save replay!");
        }

        return null;
    }
}

using System.Linq;
using fluXis.Game.Database;
using fluXis.Game.Database.Maps;
using fluXis.Game.Database.Score;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Input;
using fluXis.Game.Integration;
using fluXis.Game.Map;
using fluXis.Game.Online.API;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Online.Scores;
using fluXis.Game.Scoring;
using fluXis.Game.Screens.Result.UI;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Screens;
using osuTK;

namespace fluXis.Game.Screens.Result;

public partial class ResultsScreen : FluXisScreen, IKeyBindingHandler<FluXisKeybind>
{
    public override float Zoom => 1.2f;
    public override bool AllowMusicControl => false;

    [Resolved]
    private FluXisRealm realm { get; set; }

    [Resolved]
    private BackgroundStack backgrounds { get; set; }

    [Resolved]
    private Fluxel fluxel { get; set; }

    private readonly RealmMap map;
    private readonly MapInfo mapInfo;
    private readonly Performance performance;
    private readonly RealmScore score;

    private ResultsRatingInfo ratingInfo;

    private readonly bool showPlayData;

    public ResultsScreen(RealmMap map, MapInfo mapInfo, Performance performance, bool showPlayData = true, bool saveScore = true)
    {
        this.map = map;
        this.mapInfo = mapInfo;
        this.performance = performance;
        this.showPlayData = showPlayData;

        if (performance.IsRanked && saveScore)
        {
            score = new RealmScore(map)
            {
                Accuracy = performance.Accuracy,
                Grade = performance.Grade.ToString(),
                Score = performance.Score,
                MaxCombo = performance.MaxCombo,
                Judgements = new RealmJudgements(performance.Judgements),
                Mods = string.Join(' ', performance.Mods.Select(m => m.Acronym))
            };
        }
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;

        InternalChildren = new Drawable[]
        {
            new Container
            {
                AutoSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                CornerRadius = 10,
                Masking = true,
                EdgeEffect = new EdgeEffectParameters
                {
                    Type = EdgeEffectType.Shadow,
                    Colour = Colour4.Black.Opacity(.2f),
                    Radius = 5,
                    Offset = new Vector2(0, 2)
                },
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = FluXisColors.Background2
                    },
                    new DrawableBanner(fluxel.LoggedInUser)
                    {
                        RelativeSizeAxes = Axes.Both,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                    },
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = ColourInfo.GradientVertical(FluXisColors.Background2.Opacity(.6f), FluXisColors.Background2)
                    },
                    new FillFlowContainer
                    {
                        AutoSizeAxes = Axes.Y,
                        Width = 740,
                        Direction = FillDirection.Vertical,
                        Margin = new MarginPadding(20),
                        Children = new[]
                        {
                            new ResultTitle(map),
                            new ResultScore(performance),
                            showPlayData ? new ResultHitPoints(mapInfo, performance) : Empty(),
                            ratingInfo = new ResultsRatingInfo(showPlayData)
                        }
                    }
                }
            }
        };

        Discord.Update("Viewing Results", "", "results");

        if (score != null && !map.MapSet.Managed)
        {
            realm.RunWrite(r => r.Add(score));
            OnlineScores.UploadScore(fluxel, performance, res => ratingInfo.ScoreResponse = res);
        }
        else
            ratingInfo.ScoreResponse = new APIResponse<APIScoreResponse>(400, "Score not submittable.", null);
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisKeybind> e)
    {
        switch (e.Action)
        {
            case FluXisKeybind.Back:
                this.Exit();
                return true;
        }

        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisKeybind> e) { }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        this.ScaleTo(0.95f)
            .FadeOut()
            .ScaleTo(1f, 250, Easing.OutQuint)
            .FadeIn(250, Easing.OutQuint);

        base.OnEntering(e);
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        this.ScaleTo(1.05f, 250, Easing.OutQuint)
            .FadeOut(250, Easing.OutQuint);

        return base.OnExiting(e);
    }
}

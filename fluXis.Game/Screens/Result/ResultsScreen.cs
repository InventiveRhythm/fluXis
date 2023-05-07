using fluXis.Game.Database;
using fluXis.Game.Database.Maps;
using fluXis.Game.Database.Score;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Input;
using fluXis.Game.Integration;
using fluXis.Game.Map;
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
using osu.Framework.Logging;
using osu.Framework.Screens;
using osuTK;

namespace fluXis.Game.Screens.Result;

public partial class ResultsScreen : FluXisScreen, IKeyBindingHandler<FluXisKeybind>
{
    public override float Zoom => 1.2f;

    [Resolved]
    private FluXisRealm realm { get; set; }

    [Resolved]
    private BackgroundStack backgrounds { get; set; }

    private readonly RealmMap map;
    private readonly MapInfo mapInfo;
    private readonly Performance performance;
    private readonly RealmScore score;

    private readonly bool showHitPoints;

    public ResultsScreen(RealmMap map, MapInfo mapInfo, Performance performance, bool showHitPoints = true, bool saveScore = true)
    {
        this.map = map;
        this.mapInfo = mapInfo;
        this.performance = performance;
        this.showHitPoints = showHitPoints;

        if (performance.IsRanked && saveScore)
        {
            score = new RealmScore(map)
            {
                Accuracy = performance.Accuracy,
                Grade = performance.Grade.ToString(),
                Score = performance.Score,
                MaxCombo = performance.MaxCombo,
                Judgements = new RealmJudgements(performance.Judgements)
            };

            foreach (var mod in performance.Mods)
                score.Mods.Add(mod.Acronym);

            OnlineScores.UploadScore(performance, res => Logger.Log(res.Message));
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
                    new DrawableBanner(Fluxel.LoggedInUser)
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
                        Children = new Drawable[]
                        {
                            new ResultTitle(map),
                            new ResultScore(performance),
                            showHitPoints ? new ResultHitPoints(mapInfo, performance) : new Container()
                        }
                    }
                }
            }
        };

        Discord.Update("Viewing Results", "", "results");

        if (score != null)
            realm.RunWrite(r => r.Add(score));

        // Logger.Log(JsonConvert.SerializeObject(performance, Formatting.None));
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

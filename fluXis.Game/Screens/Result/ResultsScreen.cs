using System.Linq;
using fluXis.Game.Activity;
using fluXis.Game.Database;
using fluXis.Game.Database.Maps;
using fluXis.Game.Database.Score;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Graphics.Gamepad;
using fluXis.Game.Input;
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

    [Resolved]
    private ActivityManager activity { get; set; }

    private readonly RealmMap map;
    private readonly MapInfo mapInfo;
    private readonly Performance performance;
    private readonly RealmScore score;

    private Container content;
    private ResultsRatingInfo ratingInfo;
    private GamepadTooltipBar gamepadTooltips;

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
            content = new Container
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
            },
            gamepadTooltips = new GamepadTooltipBar
            {
                Y = 50,
                TooltipsLeft = new GamepadTooltip[]
                {
                    new()
                    {
                        Text = "Back",
                        Icon = "B"
                    }
                }
            }
        };

        activity.Update("Viewing Results", "", "results");
        GamepadHandler.OnGamepadStatusChanged += onGamepadStatusChanged;
        onGamepadStatusChanged(GamepadHandler.GamepadConnected);

        if (score != null && !map.MapSet.Managed)
        {
            realm.RunWrite(r => r.Add(score));
            OnlineScores.UploadScore(fluxel, performance, res => ratingInfo.ScoreResponse = res);
        }
        else
            ratingInfo.ScoreResponse = new APIResponse<APIScoreResponse>(400, "Score not submittable.", null);
    }

    private void onGamepadStatusChanged(bool status)
    {
        gamepadTooltips.MoveToY(status ? 0 : 50, 250, Easing.OutQuint);
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
        content.ScaleTo(0.95f).ScaleTo(1f, 250, Easing.OutQuint);
        this.FadeInFromZero(250, Easing.OutQuint);
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        onGamepadStatusChanged(false);
        content.ScaleTo(1.05f, 250, Easing.OutQuint);
        this.FadeOut(250, Easing.OutQuint);

        return false;
    }
}

using System.Linq;
using fluXis.Game.Activity;
using fluXis.Game.Database;
using fluXis.Game.Database.Maps;
using fluXis.Game.Database.Score;
using fluXis.Game.Graphics.Drawables;
using fluXis.Game.Graphics.Gamepad;
using fluXis.Game.Graphics.UserInterface.Buttons;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Input;
using fluXis.Game.Map;
using fluXis.Game.Online.API;
using fluXis.Game.Online.API.Scores;
using fluXis.Game.Online.API.Users;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Online.Scores;
using fluXis.Game.Scoring;
using fluXis.Game.Screens.Result.UI;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Screens;
using osuTK;
using osuTK.Graphics;

namespace fluXis.Game.Screens.Result;

public partial class ResultsScreen : FluXisScreen, IKeyBindingHandler<FluXisKeybind>
{
    public override float Zoom => 1.2f;
    public override bool AllowMusicControl => false;

    [Resolved]
    private FluXisRealm realm { get; set; }

    [Resolved]
    private Fluxel fluxel { get; set; }

    [Resolved]
    private ActivityManager activity { get; set; }

    private readonly RealmMap map;
    private readonly MapInfo mapInfo;
    private readonly Performance performance;
    private readonly APIUserShort player;
    private readonly RealmScore score;

    private Container content;
    private ResultsRatingInfo ratingInfo;
    private GamepadTooltipBar gamepadTooltips;
    private Container buttons;
    private CornerButton backButton;

    private readonly bool showPlayData;

    public ResultsScreen(RealmMap map, MapInfo mapInfo, Performance performance, APIUserShort player, bool showPlayData = true, bool saveScore = true)
    {
        this.map = map;
        this.mapInfo = mapInfo;
        this.performance = performance;
        this.showPlayData = showPlayData;
        this.player = player;

        if (performance.IsRanked && saveScore)
        {
            score = new RealmScore(map)
            {
                PlayerID = player?.ID ?? -1,
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
                MaskingSmoothness = 0,
                Masking = true,
                EdgeEffect = new EdgeEffectParameters
                {
                    Type = EdgeEffectType.Shadow,
                    Colour = Colour4.Black.Opacity(.25f),
                    Radius = 10,
                    Offset = new Vector2(0, 2)
                },
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = FluXisColors.Background2
                    },
                    new DrawableBanner(player)
                    {
                        RelativeSizeAxes = Axes.Both,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre
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
                            new ResultTitle(map) { User = player },
                            new ResultScore { Performance = performance },
                            showPlayData ? new ResultHitPoints { MapInfo = mapInfo, Performance = performance } : Empty(),
                            ratingInfo = new ResultsRatingInfo(showPlayData)
                        }
                    }
                }
            },
            new Container
            {
                RelativeSizeAxes = Axes.X,
                Anchor = Anchor.BottomLeft,
                Origin = Anchor.BottomLeft,
                Height = 60,
                Children = new Drawable[]
                {
                    new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Masking = true,
                        EdgeEffect = new EdgeEffectParameters
                        {
                            Type = EdgeEffectType.Shadow,
                            Colour = Color4.Black.Opacity(.25f),
                            Radius = 10
                        },
                        Child = new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = FluXisColors.Background2
                        }
                    },
                    buttons = new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Children = new Drawable[]
                        {
                            backButton = new CornerButton
                            {
                                ButtonText = "Back",
                                Icon = FontAwesome.Solid.ChevronLeft,
                                Action = this.Exit
                            }
                        }
                    },
                    gamepadTooltips = new GamepadTooltipBar
                    {
                        ShowBackground = false,
                        TooltipsLeft = new GamepadTooltip[]
                        {
                            new()
                            {
                                Text = "Back",
                                Icon = "B"
                            }
                        }
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
        gamepadTooltips.FadeTo(status ? 1 : 0);
        buttons.FadeTo(status ? 0 : 1);
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
        content.ScaleTo(0.9f).ScaleTo(1f, 400, Easing.OutQuint);
        this.FadeInFromZero(200);
        backButton.Show();
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        content.ScaleTo(1.1f, 400, Easing.OutQuint);
        this.FadeOut(200);
        backButton.Hide();

        return false;
    }
}
